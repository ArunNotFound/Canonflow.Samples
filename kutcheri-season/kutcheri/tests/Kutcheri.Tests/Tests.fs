module CanonFlow.Kutcheri.Tests

open Xunit
open FsCheck
open FsCheck.Xunit
open CanonFlow.FsCheck.Generators

[<Property>]
let ``artists form should be valid enum`` () =
    Prop.forAll artistsGenerators.arbform (fun form ->
        ["Vocal"; "Violin"; "Veena"; "Mridangam"; "Flute"; "Ensemble"] |> List.contains form
    )

[<Property>]
let ``bookings quantity should be greater than 0`` () =
    Prop.forAll bookingsGenerators.arbquantity (fun qty ->
        qty > 0
    )

[<Property>]
let ``kutcheris status should be valid enum`` () =
    Prop.forAll kutcherisGenerators.arbstatus (fun status ->
        ["draft"; "on-sale"; "confirmed"; "sold-out"; "on-hold"; "completed"; "cancelled"] |> List.contains status
    )

[<Property>]
let ``ticket tiers price should be >= 0`` () =
    Prop.forAll ticket_tiersGenerators.arbprice (fun price ->
        price >= 0
    )

[<Property>]
let ``venues seating capacity should be greater than 0`` () =
    Prop.forAll venuesGenerators.arbseating_capacity (fun capacity ->
        capacity > 0
    )
