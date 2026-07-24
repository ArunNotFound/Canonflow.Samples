namespace BankingCore.Domain

open System

type CanonflowSourceAttribute(file: string, target: string) =
    inherit Attribute()

// ==========================================
// 1. Value Objects & Refinement Types
// ==========================================
module Constants =
    [<Literal>]
    let CifLength = 8
    [<Literal>]
    let BranchCodeLength = 4
    [<Literal>]
    let CardMinLength = 16
    [<Literal>]
    let MaxPercentage = 100

module ValueObjects =
    
    type PositiveAmount = private PositiveAmount of decimal
    module PositiveAmount =
        let create (v: decimal) = 
            if v > 0m then Ok (PositiveAmount v)
            else Error "Amount must be strictly positive"
        let value (PositiveAmount v) = v

    type NonNegativeAmount = private NonNegativeAmount of decimal
    module NonNegativeAmount =
        let create (v: decimal) = 
            if v >= 0m then Ok (NonNegativeAmount v)
            else Error "Amount cannot be negative"
        let value (NonNegativeAmount v) = v

    type CifNumber = private CifNumber of string
    module CifNumber =
        let create (s: string) =
            if s.Length = Constants.CifLength then Ok (CifNumber s)
            else Error "CIF must be exactly 8 characters"
        let value (CifNumber s) = s
        
    type BranchCode = private BranchCode of string
    module BranchCode =
        let create (s: string) =
            if s.Length = Constants.BranchCodeLength then Ok (BranchCode s)
            else Error "Branch code must be exactly 4 characters"
        let value (BranchCode s) = s

    type CardNumber = private CardNumber of string
    module CardNumber =
        let create (s: string) =
            if s.Length >= Constants.CardMinLength then Ok (CardNumber s)
            else Error "Card number must be at least 16 characters"
        let value (CardNumber s) = s

    type Percentage = private Percentage of int
    module Percentage =
        let create (v: int) =
            if v > 0 && v <= Constants.MaxPercentage then Ok (Percentage v)
            else Error "Percentage must be between 1 and 100"
        let value (Percentage v) = v

// ==========================================
// 2. Enums / Discriminated Unions
// ==========================================
type AccountStatus = Open | Closed | Frozen of reason: string
type BranchStatus = Active | Closed | Suspended
type EmployeeRole = Teller | Manager | Auditor | Officer
type RiskRating = Low | Medium | High
type CustomerStatus = Active | Inactive | Dormant
type KycDocType = Passport | NationalId | DriversLicense
type AmlScreeningStatus = Cleared | Flagged | UnderReview
type FdStatus = Active | Matured | Broken
type LoanStatus = Disbursed | Closed | Defaulted
type CardType = Debit | Credit | Prepaid
type CardStatus = Active | Blocked | Expired
type EntryType = Dr | Cr
type NotificationChannel = Sms | Email | Push
type NotificationStatus = Pending | Sent | Failed

type TransactionType = Credit | Debit

// ==========================================
// 3. Entities & Aggregates
// ==========================================
type CustomerId = CustomerId of Guid
type AccountId = AccountId of Guid
type TransactionId = TransactionId of Guid
type BranchId = BranchId of Guid
type EmployeeId = EmployeeId of Guid
type CardId = CardId of Guid
type LoanId = LoanId of Guid

[<CanonflowSource("db/init/01-schema.sql", "branch")>]
type Branch = {
    Id: BranchId
    Code: ValueObjects.BranchCode
    Name: string
    Status: BranchStatus
}

[<CanonflowSource("db/init/01-schema.sql", "customer")>]
type Customer = {
    Id: CustomerId
    Cif: ValueObjects.CifNumber
    FullName: string
    DateOfBirth: DateTime
    RiskRating: RiskRating
    Status: CustomerStatus
}

type Kyc = {
    CustomerId: CustomerId
    DocumentType: KycDocType
    DocumentNumber: string
    Verified: bool
}

type Aml = {
    CustomerId: CustomerId
    ScreeningStatus: AmlScreeningStatus
    LastScreened: DateTime
}

[<CanonflowSource("db/init/01-schema.sql", "account")>]
type Account = {
    Id: AccountId
    CustomerId: CustomerId
    BranchId: BranchId
    Currency: string
    AccountType: string
    Balance: decimal 
    OverdraftLimit: ValueObjects.NonNegativeAmount
    Status: AccountStatus
}

[<CanonflowSource("db/init/01-schema.sql", "loan")>]
type Loan = {
    Id: LoanId
    AccountId: AccountId
    PrincipalAmount: ValueObjects.PositiveAmount
    OutstandingBalance: ValueObjects.NonNegativeAmount
    InterestRate: ValueObjects.NonNegativeAmount
    Status: LoanStatus
}

[<CanonflowSource("db/init/01-schema.sql", "card")>]
type Card = {
    Id: CardId
    AccountId: AccountId
    Number: ValueObjects.CardNumber
    Type: CardType
    Status: CardStatus
}

[<CanonflowSource("db/init/01-schema.sql", "transaction")>]
type Transaction = {
    Id: TransactionId
    AccountId: AccountId
    Amount: ValueObjects.PositiveAmount
    Type: TransactionType
    Currency: string
    Timestamp: DateTime
    Reference: string
}

// ==========================================
// 4. Domain Behaviors
// ==========================================
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
        | AccountStatus.Closed -> Error "Account is closed."
        | AccountStatus.Frozen reason -> Error $"Account is frozen: {reason}"
        | AccountStatus.Open ->
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
