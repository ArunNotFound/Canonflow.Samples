module SmartConstructorTests

open Xunit
open WebuyCore.Domain.ValueObjects

[<Fact>]
let ``PhoneNumber rejects invalid lengths`` () =
    match PhoneNumber.create "123" with
    | Error InvalidLength -> Assert.True(true)
    | _ -> Assert.Fail("Should reject short phone number")

[<Fact>]
let ``PhoneNumber accepts valid lengths`` () =
    match PhoneNumber.create "1234567890" with
    | Ok _ -> Assert.True(true)
    | _ -> Assert.Fail("Should accept valid phone number")

[<Fact>]
let ``Pincode rejects leading zeros`` () =
    match Pincode.create "012345" with
    | Error PincodeError.InvalidFormat -> Assert.True(true)
    | _ -> Assert.Fail("Should reject leading zeros")

[<Fact>]
let ``Pincode accepts valid format`` () =
    match Pincode.create "600001" with
    | Ok _ -> Assert.True(true)
    | _ -> Assert.Fail("Should accept valid pincode")

[<Fact>]
let ``GeoCoord bounds check`` () =
    match GeoCoord.create 100.0 0.0 with
    | Error OutOfBounds -> Assert.True(true)
    | _ -> Assert.Fail("Latitude should be within -90 to 90")

[<Fact>]
let ``Delivery ETA max limit check`` () =
    match DeliveryETA.create 150 with
    | Error InvalidETA -> Assert.True(true)
    | _ -> Assert.Fail("ETA should be max 120 mins")

[<Fact>]
let ``Discount percentage limits`` () =
    match DiscountPct.create 95.0 with
    | Error InvalidDiscount -> Assert.True(true)
    | _ -> Assert.Fail("Discount cannot exceed 90%")
