module StateMachineTests

open System
open Xunit
open WebuyCore.Domain.StateMachines
open WebuyCore.Domain.DomainModel
open WebuyCore.Domain.Enums
open WebuyCore.Domain.ValueObjects
open WebuyCore.Domain.BusinessRules

[<Fact>]
let ``Order Cancellation window strictly enforced`` () =
    let createdAt = DateTimeOffset.UtcNow.AddSeconds(-150.0)
    let order = {
        Id = Guid.NewGuid()
        ONDC_txn_id = TransactionId (Guid.NewGuid())
        ONDC_message_id = MessageId (Guid.NewGuid())
        ConsumerId = Guid.NewGuid()
        StoreId = Guid.NewGuid()
        StoreLocation = GeoCoord.create 0.0 0.0 |> Result.toOption |> Option.get
        PartnerId = None
        Items = []
        SubTotal = Money.create 100.0m |> Result.toOption |> Option.get
        DeliveryFee = Money.create 0.0m |> Result.toOption |> Option.get
        SurgeFee = Money.create 0.0m |> Result.toOption |> Option.get
        Discount = Money.create 0.0m |> Result.toOption |> Option.get
        TotalFare = Money.create 100.0m |> Result.toOption |> Option.get
        PaymentMethod = PaymentMethod.UPI
        PaymentStatus = PaymentStatus.Initiated
        Status = OrderStatus.Accepted
        OTP = OTP.create "1234" |> Result.toOption |> Option.get
        DeliveryAddress = "Mock"
        DeliveryLocation = GeoCoord.create 0.0 0.0 |> Result.toOption |> Option.get
        DeliveryETA = DeliveryETA.create 30 |> Result.toOption |> Option.get
        Distance = Distance.create 2.0 |> Result.toOption |> Option.get
        CreatedAt = createdAt
        ConfirmedAt = Some createdAt
        PackedAt = None
        PickedUpAt = None
        InTransitAt = None
        DeliveredAt = None
        CancelledAt = None
        CancellationReason = None
    }
    
    match OrderStateMachine.onCancel order DateTimeOffset.UtcNow with
    | Error (TemporalViolation _) -> Assert.True(true)
    | _ -> Assert.Fail("Cancellation should fail if > 120s has passed for Accepted orders")

[<Fact>]
let ``Fare Calculation checks free delivery threshold`` () =
    // Subtotal >= 199.0m should be free delivery
    let dummyItem = {
        ProductId = Guid.NewGuid()
        SKU = SKU.create "SKU-123" |> Result.toOption |> Option.get
        Name = "Mock"
        Quantity = Quantity.create 1 |> Result.toOption |> Option.get
        UnitPrice = Money.create 200.0m |> Result.toOption |> Option.get
        TotalPrice = Money.create 200.0m |> Result.toOption |> Option.get
        Weight = None
        StorageTemp = None
        Substitution = "NO_SUBSTITUTE"
    }
    match FareCalculation.calculate [dummyItem] 1.0 0.0m with
    | Ok (total, delivery, surge) -> 
        Assert.Equal(200.0m, total)
        Assert.Equal(0.0m, delivery)
        Assert.Equal(0.0m, surge)
    | Error _ -> Assert.Fail("Fare calculation failed")
