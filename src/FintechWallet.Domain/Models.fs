namespace FintechWallet.Domain

open System

/// Explicit transaction states as required by the DDD model
type TransactionStatus =
    | Created
    | Pending
    | Completed
    | Failed
    | Reversed

type TransactionDirection =
    | Credit
    | Debit

/// Represents an immutable entry in the financial ledger
type LedgerTransaction = {
    TransactionId: Guid
    WalletId: Guid
    Amount: decimal
    Currency: string
    Direction: TransactionDirection
    Status: TransactionStatus
    ReferenceId: string // Used for idempotency
    CreatedAt: DateTimeOffset
}

/// The Wallet acts as an aggregate root for balances
type Wallet = {
    WalletId: Guid
    CustomerId: Guid
    Currency: string
    Status: string // e.g., 'Active', 'Suspended'
    CreatedAt: DateTimeOffset
}

/// Balances are derived projections from the immutable ledger
type WalletBalance = {
    WalletId: Guid
    AvailableBalance: decimal
    LockedBalance: decimal
    UpdatedAt: DateTimeOffset
}

module WalletRules =
    /// Ensure the wallet maintains a valid balance 
    let canDebit (balance: WalletBalance) (amount: decimal) =
        balance.AvailableBalance >= amount

    let createDebit (wallet: Wallet) amount refId =
        {
            TransactionId = Guid.NewGuid()
            WalletId = wallet.WalletId
            Amount = amount
            Currency = wallet.Currency
            Direction = Debit
            Status = Created
            ReferenceId = refId
            CreatedAt = DateTimeOffset.UtcNow
        }

    let createCredit (wallet: Wallet) amount refId =
        {
            TransactionId = Guid.NewGuid()
            WalletId = wallet.WalletId
            Amount = amount
            Currency = wallet.Currency
            Direction = Credit
            Status = Created
            ReferenceId = refId
            CreatedAt = DateTimeOffset.UtcNow
        }
