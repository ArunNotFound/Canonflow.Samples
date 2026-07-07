namespace TradingCore.Domain

open System

// ==========================================
// 1. Primitive Value Objects & Types
// ==========================================
type AccountId = AccountId of Guid
type OrderId = OrderId of Guid

// Note how we don't have to write "PositiveQuantity" or "ValidTicker" types here.
// The Database and CanonFlow's TS generated boundaries handle the structural types.
// We just use primitives and focus on the Verbs.

type TradeSide = | Buy | Sell

type TradeOrder = {
    Id: OrderId
    AccountId: AccountId
    Ticker: string
    Side: TradeSide
    Quantity: int
    LimitPrice: decimal
}

type Account = {
    Id: AccountId
    CashBalance: decimal
    TotalAccountValue: decimal // Cash + Stock Value combined
}

module MarketRules =
    // APP CONSTRAINT 1: Market Hours
    // This is impossible to enforce cleanly in a Database CHECK constraint
    // because it relies on the dynamic clock and holiday schedules.
    // It belongs in the F# App.
    let isMarketOpen (currentTime: DateTimeOffset) =
        let est = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(currentTime, "Eastern Standard Time")
        let timeOfDay = est.TimeOfDay
        let marketOpen = TimeSpan(9, 30, 0)
        let marketClose = TimeSpan(16, 0, 0)
        
        est.DayOfWeek <> DayOfWeek.Saturday && 
        est.DayOfWeek <> DayOfWeek.Sunday &&
        timeOfDay >= marketOpen && 
        timeOfDay <= marketClose

    // APP CONSTRAINT 2: Pattern Day Trader (PDT) Rule
    // Requires historical queries and aggregate states. A Database CHECK constraint
    // cannot examine a 5-day rolling window of external rows dynamically.
    // It belongs in the F# App.
    let violatesPdtRule (account: Account) (recentDayTradesCount: int) =
        account.TotalAccountValue < 25000m && recentDayTradesCount >= 3


// ==========================================
// 2. The DDD Verb (Hand in Hand with DB)
// ==========================================
module TradingBehavior =
    type Command =
        | SubmitOrder of TradeOrder * currentTime: DateTimeOffset * recentDayTrades: int

    type Event =
        | OrderAccepted of OrderId
        | OrderRejected of reason: string

    let execute (cmd: Command) (account: Account) : Event =
        match cmd with
        | SubmitOrder (order, currentTime, recentDayTrades) ->
            
            // 1. Evaluate App-Side Constraint (Market Hours)
            if not (MarketRules.isMarketOpen currentTime) then
                OrderRejected "Market is currently closed."
                
            // 2. Evaluate App-Side Constraint (PDT Rule)
            elif MarketRules.violatesPdtRule account recentDayTrades then
                OrderRejected "PDT Rule Violation: Account value under $25,000 cannot exceed 3 day trades."
                
            // 3. Evaluate App-Side Constraint (Business Logic: Sufficient Funds)
            elif order.Side = Buy && (decimal order.Quantity * order.LimitPrice) > account.CashBalance then
                OrderRejected "Insufficient buying power."
                
            else
                // 4. Success!
                // We emit the event. When it persists to the DB, the DB will physically enforce
                // the structural limits (e.g., if somehow a negative quantity slipped in, the DB blocks it).
                OrderAccepted order.Id
