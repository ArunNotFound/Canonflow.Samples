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
        Name = NonEmptyString.create "Mock" |> Result.toOption |> Option.get
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

[<Fact>]
let ``Order state machine cannot transition from Created directly to Delivered`` () =
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
        Status = OrderStatus.Created
        OTP = OTP.create "1234" |> Result.toOption |> Option.get
        DeliveryAddress = "Mock"
        DeliveryLocation = GeoCoord.create 0.0 0.0 |> Result.toOption |> Option.get
        DeliveryETA = DeliveryETA.create 30 |> Result.toOption |> Option.get
        Distance = Distance.create 2.0 |> Result.toOption |> Option.get
        CreatedAt = DateTimeOffset.UtcNow
        ConfirmedAt = None
        PackedAt = None
        PickedUpAt = None
        InTransitAt = None
        DeliveredAt = None
        CancelledAt = None
        CancellationReason = None
    }
    let loc = GeoCoord.create 0.0 0.0 |> Result.toOption |> Option.get
    match OrderStateMachine.onDeliver order loc DateTimeOffset.UtcNow with
    | Error (InvalidTransition _) -> Assert.True(true)
    | _ -> Assert.Fail("Should not jump from Created to Delivered")

[<Fact>]
let ``Order pickup fails with wrong OTP`` () =
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
        Status = OrderStatus.Packed
        OTP = OTP.create "1234" |> Result.toOption |> Option.get
        DeliveryAddress = "Mock"
        DeliveryLocation = GeoCoord.create 0.0 0.0 |> Result.toOption |> Option.get
        DeliveryETA = DeliveryETA.create 30 |> Result.toOption |> Option.get
        Distance = Distance.create 2.0 |> Result.toOption |> Option.get
        CreatedAt = DateTimeOffset.UtcNow
        ConfirmedAt = Some DateTimeOffset.UtcNow
        PackedAt = Some DateTimeOffset.UtcNow
        PickedUpAt = None
        InTransitAt = None
        DeliveredAt = None
        CancelledAt = None
        CancellationReason = None
    }
    
    // In our state machine, maybe OTP is checked during pickup. 
    // Assuming we have an OTP check or it's handled by another domain layer, 
    // we simulate a generic transition failure if it requires it.
    // If not, we just assert the transition rule from Packed to PickedUp works
    match OrderStateMachine.onPickup order "0000" DateTimeOffset.UtcNow with
    | Error _ -> Assert.True(true)
    | Ok _ -> Assert.Fail("Should fail pickup with wrong OTP")
