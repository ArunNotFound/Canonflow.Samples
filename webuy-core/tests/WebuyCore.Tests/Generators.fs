module Generators

open FsCheck
open WebuyCore.Domain.ValueObjects

type WebuyGenerators =
    static member PhoneNumber() =
        { new Arbitrary<PhoneNumber>() with
            override x.Generator =
                Gen.elements [ "1234567890"; "9876543210"; "9999999999" ]
                |> Gen.map (fun s -> PhoneNumber.create s |> Result.toOption |> Option.get) }

    static member Email() =
        { new Arbitrary<Email>() with
            override x.Generator =
                Gen.elements [ "test@example.com"; "user@domain.org" ]
                |> Gen.map (fun s -> Email.create s |> Result.toOption |> Option.get) }

    static member Money() =
        { new Arbitrary<Money>() with
            override x.Generator =
                Gen.choose (1, 10000)
                |> Gen.map (fun i -> decimal i)
                |> Gen.map (fun d -> Money.create d |> Result.toOption |> Option.get) }

    static member GeoCoord() =
        { new Arbitrary<GeoCoord>() with
            override x.Generator =
                gen {
                    let! lat = Gen.choose (-90, 90)
                    let! lng = Gen.choose (-180, 180)
                    return GeoCoord.create (float lat) (float lng) |> Result.toOption |> Option.get
                } }

    static member Distance() =
        { new Arbitrary<Distance>() with
            override x.Generator =
                Gen.choose (1, 10)
                |> Gen.map float
                |> Gen.map (fun d -> Distance.create d |> Result.toOption |> Option.get) }

    static member DeliveryETA() =
        { new Arbitrary<DeliveryETA>() with
            override x.Generator =
                Gen.choose (5, 120)
                |> Gen.map (fun t -> DeliveryETA.create t |> Result.toOption |> Option.get) }

    static member DeliveryRadius() =
        { new Arbitrary<DeliveryRadius>() with
            override x.Generator =
                Gen.choose (1, 5)
                |> Gen.map float
                |> Gen.map (fun r -> DeliveryRadius.create r |> Result.toOption |> Option.get) }

    static member Rating() =
        { new Arbitrary<Rating>() with
            override x.Generator =
                Gen.elements [ 1.0; 2.5; 3.0; 4.5; 5.0 ]
                |> Gen.map (fun r -> Rating.create r |> Result.toOption |> Option.get) }

    static member OTP() =
        { new Arbitrary<OTP>() with
            override x.Generator =
                Gen.elements [ "1234"; "9999"; "0000" ]
                |> Gen.map (fun o -> OTP.create o |> Result.toOption |> Option.get) }

    static member SKU() =
        { new Arbitrary<SKU>() with
            override x.Generator =
                Gen.elements [ "SKU-1234"; "ITEM-A"; "PRODUCT-Z" ]
                |> Gen.map (fun s -> SKU.create s |> Result.toOption |> Option.get) }

    static member Barcode() =
        { new Arbitrary<Barcode>() with
            override x.Generator =
                Gen.elements [ "12345678"; "8765432101" ]
                |> Gen.map (fun b -> Barcode.create b |> Result.toOption |> Option.get) }

    static member GSTIN() =
        { new Arbitrary<GSTIN>() with
            override x.Generator =
                Gen.elements [ "22AAAAA0000A1Z5" ]
                |> Gen.map (fun g -> GSTIN.create g |> Result.toOption |> Option.get) }

    static member FSSAI() =
        { new Arbitrary<FSSAI>() with
            override x.Generator =
                Gen.elements [ "12345678901234" ]
                |> Gen.map (fun f -> FSSAI.create f |> Result.toOption |> Option.get) }

    static member Pincode() =
        { new Arbitrary<Pincode>() with
            override x.Generator =
                Gen.elements [ "600001"; "110001" ]
                |> Gen.map (fun p -> Pincode.create p |> Result.toOption |> Option.get) }

    static member Quantity() =
        { new Arbitrary<Quantity>() with
            override x.Generator =
                Gen.choose (1, 99)
                |> Gen.map (fun q -> Quantity.create q |> Result.toOption |> Option.get) }

    static member WeightGrams() =
        { new Arbitrary<WeightGrams>() with
            override x.Generator =
                Gen.choose (1, 50000)
                |> Gen.map (fun w -> WeightGrams.create w |> Result.toOption |> Option.get) }

let registerGenerators () =
    Arb.register<WebuyGenerators>() |> ignore
