module BankingCore.Tests

open Expecto
open FsCheck
open CanonFlow.FsCheck.Generators

let properties =
    testList "BankingCore Properties" [
        testProperty "account type should be valid enum" (fun (accountType: string) ->
            Prop.forAll accountGenerators.arbaccount_type (fun at ->
                ["SAVINGS"; "CURRENT"; "LOAN"; "FD"] |> List.contains at
            )
        )
        
        testProperty "customer risk rating should be valid enum" (fun (riskRating: string) ->
            Prop.forAll customerGenerators.arbrisk_rating (fun rr ->
                ["LOW"; "MEDIUM"; "HIGH"] |> List.contains rr
            )
        )
        
        testProperty "transaction type should be CREDIT or DEBIT" (fun (txnType: string) ->
            Prop.forAll transactionGenerators.arbtxn_type (fun tt ->
                ["CREDIT"; "DEBIT"] |> List.contains tt
            )
        )
        
        testProperty "charge amount should be greater than 0" (fun (amount: decimal) ->
            Prop.forAll chargesGenerators.arbamount (fun a ->
                a > 0
            )
        )
    ]
