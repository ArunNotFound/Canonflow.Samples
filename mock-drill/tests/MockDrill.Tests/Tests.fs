module CanonFlow.MockDrill.Tests

open Xunit
open FsCheck
open FsCheck.Xunit
open CanonFlow.FsCheck.Generators

[<Property>]
let ``customers age should always be between 18 and 119`` () =
    Prop.forAll customersGenerators.arbage (fun age -> 
        age >= 18 && age < 120
    )

[<Property>]
let ``orders amount should always be greater than 0`` () =
    Prop.forAll ordersGenerators.arbamount (fun amount ->
        amount > 0
    )

[<Property>]
let ``customers status should be valid enum`` () =
    Prop.forAll customersGenerators.arbstatus (fun status ->
        ["ACTIVE"; "SUSPENDED"; "CLOSED"] |> List.contains status
    )

[<Property>]
let ``orders currency should be valid enum`` () =
    Prop.forAll ordersGenerators.arbcurrency (fun currency ->
        ["INR"; "USD"; "EUR"] |> List.contains currency
    )
