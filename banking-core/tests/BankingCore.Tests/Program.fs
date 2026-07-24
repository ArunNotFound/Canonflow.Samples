open System
open Expecto
open FsCheck
open BankingCore.Domain

module LegacyDomain =
    // The "Without FsAssay" approach (Primitive Obsession)
    type Account = {
        Id: Guid
        Balance: decimal
        OverdraftLimit: decimal
        BranchCode: string // Should be 4 chars, but string allows anything
    }

    let withdraw (amount: decimal) (account: Account) =
        // BUG: In legacy code, a negative amount might slip through!
        // Withdraw(-500) will actually DEPOSIT 500.
        let newBalance = account.Balance - amount
        if newBalance >= -account.OverdraftLimit then
            Ok { account with Balance = newBalance }
        else
            Error "Insufficient funds"

module Tests =

    let legacyTests =
        testList "Legacy Primitive Obsession (Without FsAssay)" [
            testProperty "Legacy withdrawal is vulnerable to negative amounts (Exploited by FsCheck!)" <| fun (balance: decimal, limit: decimal, amount: decimal) ->
                // FsCheck will eventually generate a negative amount.
                // Let's restrict to non-negative initial balances just to set the stage.
                if balance >= 0m && limit >= 0m && amount < 0m then
                    let account = { LegacyDomain.Id = Guid.NewGuid(); LegacyDomain.Balance = balance; LegacyDomain.OverdraftLimit = limit; LegacyDomain.BranchCode = "US01" }
                    match LegacyDomain.withdraw amount account with
                    | Ok newAccount ->
                        // The exploit! A negative withdrawal increased the balance!
                        Expect.isTrue (newAccount.Balance > account.Balance) "A negative withdrawal shouldn't increase the balance, but it did!"
                    | Error _ -> ()
        ]

    let fsAssayTests =
        testList "Uplifted Domain (With FsAssay)" [
            
            testProperty "FsAssay blocks negative withdrawals at the type level" <| fun (amount: decimal) ->
                // FsAssay forces us to use PositiveAmount.
                // If FsCheck generates a negative number or zero, creation fails immediately.
                if amount <= 0m then
                    let result = ValueObjects.PositiveAmount.create amount
                    Expect.isError result "Negative or zero amounts must be rejected by the refinement type!"
                else
                    let result = ValueObjects.PositiveAmount.create amount
                    Expect.isOk result "Positive amounts must be accepted."

            testProperty "FsAssay BranchCode must be exactly 4 characters" <| fun (code: string) ->
                let result = ValueObjects.BranchCode.create (if isNull code then "" else code)
                if not (isNull code) && code.Length = Constants.BranchCodeLength then
                    Expect.isOk result "Valid branch code should be accepted"
                else
                    Expect.isError result "Invalid branch code should be rejected"
                    
            testProperty "FsAssay withdrawals only execute safely with PositiveAmount" <| fun (balance: decimal, limit: decimal, amount: decimal) ->
                // FsCheck generates random primitives. We must parse them through FsAssay smart constructors.
                let amountResult = ValueObjects.PositiveAmount.create amount
                let limitResult = ValueObjects.NonNegativeAmount.create limit
                
                match amountResult, limitResult with
                | Ok positiveAmt, Ok nonNegLimit ->
                    let account = { 
                        Id = AccountId(Guid.NewGuid())
                        CustomerId = CustomerId(Guid.NewGuid())
                        BranchId = BranchId(Guid.NewGuid())
                        Currency = "USD"
                        AccountType = "Checking"
                        Balance = balance 
                        OverdraftLimit = nonNegLimit
                        Status = AccountStatus.Open 
                    }
                    
                    let cmd = AccountBehavior.AccountCommand.Withdraw positiveAmt
                    let res = AccountBehavior.execute cmd account
                    
                    let withdrawVal = ValueObjects.PositiveAmount.value positiveAmt
                    let overdraftLimit = ValueObjects.NonNegativeAmount.value nonNegLimit
                    
                    if balance - withdrawVal >= -overdraftLimit then
                        Expect.isOk res "Should succeed if within overdraft"
                    else
                        Expect.isError res "Should fail if exceeding overdraft"
                | _ -> 
                    // FsAssay protected the domain from invalid test data!
                    ()
        ]

[<EntryPoint>]
let main argv =
    let tests = testList "BankingCore Domain Tests" [ Tests.legacyTests; Tests.fsAssayTests; BankingCore.Tests.properties ]
    runTestsWithCLIArgs [] argv tests
