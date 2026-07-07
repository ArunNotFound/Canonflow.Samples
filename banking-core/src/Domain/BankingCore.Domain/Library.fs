namespace BankingCore.Domain

open System

// ==========================================
// 1. Value Objects & Refinement Types
// ==========================================
// Surprise #1: Boilerplate validation in code for simple SQL constraints
module ValueObjects =
    
    // Equivalent to: CHECK (amount > 0)
    type PositiveAmount = private PositiveAmount of decimal
    module PositiveAmount =
        let create (v: decimal) = 
            if v > 0m then Ok (PositiveAmount v)
            else Error "Amount must be strictly positive"
        let value (PositiveAmount v) = v

    // Equivalent to: CHECK (amount >= 0)
    type NonNegativeAmount = private NonNegativeAmount of decimal
    module NonNegativeAmount =
        let create (v: decimal) = 
            if v >= 0m then Ok (NonNegativeAmount v)
            else Error "Amount cannot be negative"
        let value (NonNegativeAmount v) = v

    // Equivalent to: CHECK (length(cif_number) = 8)
    type CifNumber = private CifNumber of string
    module CifNumber =
        let create (s: string) =
            if s.Length = 8 then Ok (CifNumber s)
            else Error "CIF must be exactly 8 characters"
        let value (CifNumber s) = s

    // Equivalent to: CHECK (allocation_percentage > 0 AND allocation_percentage <= 100)
    type Percentage = private Percentage of int
    module Percentage =
        let create (v: int) =
            if v > 0 && v <= 100 then Ok (Percentage v)
            else Error "Percentage must be between 1 and 100"
        let value (Percentage v) = v

// ==========================================
// 2. Enums / Discriminated Unions
// ==========================================
// Surprise #2: F# DUs are behaviorally richer but don't map cleanly to raw SQL strings
// without boilerplate mapping functions.
type AccountStatus = 
    | Open 
    | Closed 
    | Frozen of reason: string // SQL didn't capture the 'reason' cleanly without a separate column!

type RiskRating = Low | Medium | High
type TransactionType = Credit | Debit

// ==========================================
// 3. Entities & Aggregates
// ==========================================
type CustomerId = CustomerId of Guid
type AccountId = AccountId of Guid
type TransactionId = TransactionId of Guid

type Customer = {
    Id: CustomerId
    Cif: ValueObjects.CifNumber
    FullName: string
    DateOfBirth: DateTime // Code must validate < DateTime.Today upon creation
    RiskRating: RiskRating
}

type Transaction = {
    Id: TransactionId
    AccountId: AccountId
    Amount: ValueObjects.PositiveAmount
    Type: TransactionType
    Timestamp: DateTime
}

type Account = {
    Id: AccountId
    CustomerId: CustomerId
    Balance: decimal // Wait! Balance shouldn't just be decimal, but we can't restrict it easily if overdrafts are allowed.
    OverdraftLimit: ValueObjects.NonNegativeAmount
    Status: AccountStatus
}

// ==========================================
// 4. Domain Behaviors (The real difference)
// ==========================================
// Surprise #3: Database was state-centric (CRUD). DDD is behavior-centric (Commands/Events).
module AccountBehavior =
    type AccountCommand =
        | Deposit of amount: ValueObjects.PositiveAmount
        | Withdraw of amount: ValueObjects.PositiveAmount

    type AccountEvent =
        | Deposited of amount: decimal
        | Withdrawn of amount: decimal
        | WithdrawalFailed of reason: string

    let execute (cmd: AccountCommand) (account: Account) : Result<AccountEvent list, string> =
        match account.Status with
        | Closed -> Error "Account is closed."
        | Frozen reason -> Error $"Account is frozen: {reason}"
        | Open ->
            match cmd with
            | Deposit amount -> 
                Ok [ Deposited (ValueObjects.PositiveAmount.value amount) ]
            | Withdraw amount ->
                let withdrawVal = ValueObjects.PositiveAmount.value amount
                let overdraftLimit = ValueObjects.NonNegativeAmount.value account.OverdraftLimit
                if (account.Balance - withdrawVal) >= -overdraftLimit then
                    Ok [ Withdrawn withdrawVal ]
                else
                    Error "Insufficient funds and overdraft limit exceeded."
