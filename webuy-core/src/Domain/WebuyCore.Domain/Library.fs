namespace WebuyCore.Domain

open System
open System.Text.RegularExpressions

module ValueObjects =

    type PhoneNumberError = InvalidLength | NotDigits
    type PhoneNumber = private PhoneNumber of string
    module PhoneNumber =
        let value (PhoneNumber p) = p
        let create (p: string) =
            if isNull p then Error InvalidLength
            elif p.Length < 10 || p.Length > 15 then Error InvalidLength
            elif not (p |> Seq.forall Char.IsDigit) then Error NotDigits
            else Ok (PhoneNumber p)

    type EmailError = InvalidFormat
    type Email = private Email of string
    module Email =
        let value (Email e) = e
        let create (e: string) =
            if isNull e then Error InvalidFormat
            elif e.Contains("@") && e.Contains(".") && not (e.Contains(" ")) then Ok (Email e)
            else Error InvalidFormat

    type PincodeError = InvalidFormat
    type Pincode = private Pincode of string
    module Pincode =
        let value (Pincode p) = p
        let create (p: string) =
            if isNull p || p.Length <> 6 then Error InvalidFormat
            elif p.[0] = '0' then Error InvalidFormat
            elif not (p |> Seq.forall Char.IsDigit) then Error InvalidFormat
            else Ok (Pincode p)

    type GSTINError = InvalidFormat
    type GSTIN = private GSTIN of string
    module GSTIN =
        let value (GSTIN g) = g
        let create (g: string) =
            let regex = Regex("^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z][0-9][A-Z0-9]Z[0-9A-Z]$")
            if isNull g || not (regex.IsMatch(g)) then Error InvalidFormat
            else Ok (GSTIN g)

    type FSSAIError = InvalidFormat
    type FSSAI = private FSSAI of string
    module FSSAI =
        let value (FSSAI f) = f
        let create (f: string) =
            if isNull f || f.Length <> 14 then Error InvalidFormat
            elif not (f |> Seq.forall Char.IsDigit) then Error InvalidFormat
            else Ok (FSSAI f)

    type GeoCoordError = OutOfBounds
    type GeoCoord = private GeoCoord of float * float
    module GeoCoord =
        let value (GeoCoord(lat, lng)) = (lat, lng)
        let create lat lng =
            if lat < -90.0 || lat > 90.0 then Error OutOfBounds
            elif lng < -180.0 || lng > 180.0 then Error OutOfBounds
            else Ok (GeoCoord(lat, lng))

    type DistanceError = InvalidDistance
    type Distance = private Distance of float
    module Distance =
        let value (Distance d) = d
        let create d =
            if d <= 0.0 || d > 10.0 then Error InvalidDistance
            else Ok (Distance d)

    type DeliveryRadiusError = InvalidRadius
    type DeliveryRadius = private DeliveryRadius of float
    module DeliveryRadius =
        let value (DeliveryRadius r) = r
        let create r =
            if r < 0.5 || r > 5.0 then Error InvalidRadius
            else Ok (DeliveryRadius r)

    type DeliveryETAError = InvalidETA
    type DeliveryETA = private DeliveryETA of int
    module DeliveryETA =
        let value (DeliveryETA t) = t
        let create t =
            if t < 5 || t > 120 then Error InvalidETA
            else Ok (DeliveryETA t)

    type MoneyError = Negative | InvalidPrecision
    type Money = private Money of decimal
    module Money =
        let value (Money m) = m
        let create (m: decimal) =
            if m < 0.0m then Error Negative
            elif Math.Round(m, 2) <> m then Error InvalidPrecision
            else Ok (Money m)

    type QuantityError = InvalidQuantity
    type Quantity = private Quantity of int
    module Quantity =
        let value (Quantity q) = q
        let create q =
            if q < 1 || q > 99 then Error InvalidQuantity
            else Ok (Quantity q)

    type WeightGramsError = InvalidWeight
    type WeightGrams = private WeightGrams of int
    module WeightGrams =
        let value (WeightGrams w) = w
        let create w =
            if w < 1 || w > 50000 then Error InvalidWeight
            else Ok (WeightGrams w)

    type DiscountPctError = InvalidDiscount
    type DiscountPct = private DiscountPct of float
    module DiscountPct =
        let value (DiscountPct d) = d
        let create d =
            if d < 0.0 || d > 90.0 then Error InvalidDiscount
            else Ok (DiscountPct d)

    type SurgeMultiplierError = InvalidMultiplier
    type SurgeMultiplier = private SurgeMultiplier of float
    module SurgeMultiplier =
        let value (SurgeMultiplier s) = s
        let create s =
            if s < 1.0 || s > 3.0 then Error InvalidMultiplier
            else Ok (SurgeMultiplier s)

    type SKUError = InvalidSKU
    type SKU = private SKU of string
    module SKU =
        let value (SKU s) = s
        let create (s: string) =
            if isNull s || s.Length < 6 || s.Length > 30 then Error InvalidSKU
            elif not (Regex.IsMatch(s, "^[A-Z0-9-]+$")) then Error InvalidSKU
            else Ok (SKU s)

    type BarcodeError = InvalidBarcode
    type Barcode = private Barcode of string
    module Barcode =
        let value (Barcode b) = b
        let create (b: string) =
            if isNull b || b.Length < 8 || b.Length > 14 then Error InvalidBarcode
            elif not (b |> Seq.forall Char.IsDigit) then Error InvalidBarcode
            else Ok (Barcode b)

    type ExpiryDateError = InvalidDate
    type ExpiryDate = private ExpiryDate of DateTime
    module ExpiryDate =
        let value (ExpiryDate d) = d
        let create (d: DateTime) (today: DateTime) =
            if d <= today then Error InvalidDate
            else Ok (ExpiryDate d)

    type ShelfLifeDaysError = InvalidShelfLife
    type ShelfLifeDays = private ShelfLifeDays of int
    module ShelfLifeDays =
        let value (ShelfLifeDays d) = d
        let create d =
            if d < 1 || d > 3650 then Error InvalidShelfLife
            else Ok (ShelfLifeDays d)

    type TemperatureCelsiusError = InvalidTemperature
    type TemperatureCelsius = private TemperatureCelsius of float
    module TemperatureCelsius =
        let value (TemperatureCelsius t) = t
        let create t =
            if t < -25.0 || t > 60.0 then Error InvalidTemperature
            else Ok (TemperatureCelsius t)

    type RatingError = InvalidRating
    type Rating = private Rating of float
    module Rating =
        let value (Rating r) = r
        let create r =
            if r < 1.0 || r > 5.0 || Math.Round(r, 1) <> r then Error InvalidRating
            else Ok (Rating r)

    type OTPError = InvalidOTP
    type OTP = private OTP of string
    module OTP =
        let value (OTP o) = o
        let create (o: string) =
            if isNull o || o.Length <> 4 || not (o |> Seq.forall Char.IsDigit) then Error InvalidOTP
            else Ok (OTP o)

    type TransactionId = TransactionId of Guid
    type MessageId = MessageId of Guid

    type SubscriberIdError = InvalidSubscriberId
    type SubscriberId = private SubscriberId of string
    module SubscriberId =
        let value (SubscriberId s) = s
        let create (s: string) =
            if isNull s || not (Regex.IsMatch(s, "^[a-z0-9.]+$")) then Error InvalidSubscriberId
            else Ok (SubscriberId s)

module Enums =
    type Category =
        | Fruits | Vegetables | Dairy | Bakery | Snacks | Beverages
        | AttaRice | OilsMasala | PersonalCare | Cleaning
        | BabyCare | PetFood | Electronics | Beauty
        | Pharmacy | Kitchen | Puja | Stationery
        | Printouts | HomeOffice | IceCream | Frozen

    type Unit = Piece | Kg | Gram | Litre | Ml | Pack | Dozen | Bundle | Sheet
    
    type StoreStatus = Active | Inactive | Maintenance
    
    type ConsumerStatus = Active | Banned | Unverified
    
    type PartnerStatus = Registered | KYCPending | KYCVerified | Online | Assigned | Picking | Delivering | Offline | Suspended | Deactivated
    
    type OrderStatus = Created | Accepted | Packed | PickedUp | InTransit | Delivered | Cancelled | Returned | Refunded
    
    type PaymentMethod = UPI | Card | Wallet | COD | NetBanking | BNPL | Sodexo | PaytmFood | WebuyWallet
    
    type PaymentStatus = Initiated | Authorized | Captured | Settled | Failed | RefundInitiated | Refunded
    
    type Frequency = Daily | AlternateDays | Weekly | BiWeekly | Monthly
    
    type SubscriptionStatus = Active | Paused | Skipped | Cancelled | Expired
    
    type ColorMode = BW | Color
    
    type PaperSize = A4 | A3 | Letter
    
    type City = 
        | Ahmedabad | Bengaluru | Chandigarh | Chennai | Delhi 
        | Faridabad | Gurgaon | Hyderabad | Jaipur | Jalandhar 
        | Kanpur | Kolkata | Lucknow | Ludhiana | Meerut 
        | Mohali | Mumbai | Panchkula | Pune | Noida 
        | Ghaziabad | Vadodara | Zirakpur
        
    type BecknAPI = Search | Select | Init | Confirm | Status | Track | Cancel | Update | Rating | Support
    
    type ONDCLifecycleState = Searching | Selected | Initializing | Confirmed | Active | Completed | ONDCCancelled
    
    type InventoryStatus = InStock | LowStock | Reserved | OutOfStock | Restocking | Expired | Discontinued

module DomainModel =
    open ValueObjects
    open Enums

    type NonEmptyString = string // placeholder for smart constructor
    type Address = string // placeholder
    type SubCategory = string
    type TimeRange = { Start: TimeSpan; End: TimeSpan }
    type Capacity = int
    type Duration = TimeSpan
    type VehicleType = string
    type VehicleReg = string
    type Speed = float
    type KYCStatus = string
    type SubstitutionPolicy = string
    type CancellationReason = string
    type Preferences = string
    type PrintDocument = string

    type Product = {
        Id: Guid
        SKU: SKU
        Barcode: Barcode option
        Name: NonEmptyString
        Category: Category
        SubCategory: SubCategory
        Brand: NonEmptyString
        MRP: Money
        SellingPrice: Money
        Discount: DiscountPct
        Unit: Unit
        Weight: WeightGrams option
        Images: Uri list
        Description: string
        Expiry: ExpiryDate option
        StorageTemp: TemperatureCelsius option
        IsFSSAI: bool
        FSSAILicense: FSSAI option
        Tags: string list
        IsActive: bool
        CreatedAt: DateTimeOffset
    }

    type InventoryItem = {
        ProductId: Guid
        StoreId: Guid
        Stock: Quantity
        Reserved: Quantity
        Available: Quantity
        ShelfLocation: string
        ExpiryBatch: ExpiryDate option
        StorageTemp: TemperatureCelsius option
        LastRestocked: DateTimeOffset
        Status: InventoryStatus
    }
    
    module InventoryItem =
        let validate (item: InventoryItem) =
            let stock = Quantity.value item.Stock
            let reserved = Quantity.value item.Reserved
            let available = Quantity.value item.Available
            if available = stock - reserved && available >= 0 then Ok item
            else Error "Invariant violated: Available must equal Stock - Reserved and be >= 0"

    type DarkStore = {
        Id: Guid
        Name: NonEmptyString
        Location: GeoCoord
        Address: Address
        Pincode: Pincode
        City: City
        DeliveryRadius: DeliveryRadius
        OperatingHours: TimeRange
        Capacity: Capacity
        ColdStorage: bool
        FSSAILicense: FSSAI option
        GSTIN: GSTIN
        Status: StoreStatus
        Inventory: InventoryItem list
        AvgPickTime: Duration
        Rating: Rating
    }

    type SubscriptionItem = string // placeholder

    type Subscription = {
        Id: Guid
        ConsumerId: Guid
        Items: SubscriptionItem list
        Frequency: Frequency
        DeliveryTime: TimeRange
        NextDelivery: DateTimeOffset
        Status: SubscriptionStatus
        PaymentMethod: PaymentMethod
        PauseUntil: DateTimeOffset option
    }

    type Consumer = {
        Id: Guid
        Phone: PhoneNumber
        Email: Email option
        Name: NonEmptyString
        Addresses: Address list
        DefaultAddress: Address
        Wallet: Money
        Subscription: Subscription option
        Preferences: Preferences
        Status: ConsumerStatus
        CreatedAt: DateTimeOffset
    }

    type DeliveryPartner = {
        Id: Guid
        Phone: PhoneNumber
        Name: NonEmptyString
        VehicleType: VehicleType
        VehicleReg: VehicleReg
        CurrentLocation: GeoCoord
        AssignedStore: Guid option
        Status: PartnerStatus
        Rating: Rating
        ActiveOrders: Guid list
        MaxConcurrent: Quantity
        SpeedKmph: float // Speed
        Earnings: Money
        KYC: KYCStatus
    }

    module DeliveryPartner =
        let validate (partner: DeliveryPartner) =
            if partner.SpeedKmph <= 20.0 then Ok partner
            else Error "Invariant violated: SpeedKmph must be <= 20"

    type OrderItem = {
        ProductId: Guid
        SKU: SKU
        Name: NonEmptyString
        Quantity: Quantity
        UnitPrice: Money
        TotalPrice: Money
        Weight: WeightGrams option
        StorageTemp: TemperatureCelsius option
        Substitution: SubstitutionPolicy
    }

    type Order = {
        Id: Guid
        ONDC_txn_id: TransactionId
        ONDC_message_id: MessageId
        ConsumerId: Guid
        StoreId: Guid
        StoreLocation: GeoCoord
        PartnerId: Guid option
        Items: OrderItem list
        SubTotal: Money
        DeliveryFee: Money
        SurgeFee: Money
        Discount: Money
        TotalFare: Money
        PaymentMethod: PaymentMethod
        PaymentStatus: PaymentStatus
        Status: OrderStatus
        OTP: OTP
        DeliveryAddress: Address
        DeliveryLocation: GeoCoord
        DeliveryETA: DeliveryETA
        Distance: Distance
        CreatedAt: DateTimeOffset
        ConfirmedAt: DateTimeOffset option
        PackedAt: DateTimeOffset option
        PickedUpAt: DateTimeOffset option
        InTransitAt: DateTimeOffset option
        DeliveredAt: DateTimeOffset option
        CancelledAt: DateTimeOffset option
        CancellationReason: CancellationReason option
    }

    type PrintoutOrder = {
        Id: Guid
        ConsumerId: Guid
        Documents: PrintDocument list
        ColorMode: ColorMode
        PaperSize: PaperSize
        Copies: Quantity
        SingleSided: bool
        TotalPages: Quantity
        TotalCost: Money
        Status: OrderStatus
    }

module DistanceUtil =
    open ValueObjects
    let calculate (c1: GeoCoord) (c2: GeoCoord) = 0.05 // Mock implementation returning 50 meters

module StateMachines =
    open DomainModel
    open Enums
    open ValueObjects

    type StateError =
        | InvalidTransition of string
        | PreconditionFailed of string
        | TemporalViolation of string

    // Σ1: Order Lifecycle
    module OrderStateMachine =
        let onConfirm (order: Order) (now: DateTimeOffset) =
            match order.Status with
            | Created -> Ok { order with Status = Accepted; ConfirmedAt = Some now }
            | _ -> Error (InvalidTransition "Can only confirm from Created")

        let onPack (order: Order) (now: DateTimeOffset) =
            match order.Status with
            | Accepted -> Ok { order with Status = Packed; PackedAt = Some now }
            | _ -> Error (InvalidTransition "Can only pack from Accepted")

        let onPickup (order: Order) (partnerOtp: string) (now: DateTimeOffset) =
            match order.Status with
            | Packed -> 
                if ValueObjects.OTP.value order.OTP = partnerOtp then
                    Ok { order with Status = PickedUp; PickedUpAt = Some now }
                else Error (PreconditionFailed "OTP Mismatch")
            | _ -> Error (InvalidTransition "Can only pickup from Packed")

        let onTransit (order: Order) (now: DateTimeOffset) =
            match order.Status with
            | PickedUp -> Ok { order with Status = InTransit; InTransitAt = Some now }
            | _ -> Error (InvalidTransition "Can only transit from PickedUp")

        let onDeliver (order: Order) (partnerLocation: GeoCoord) (now: DateTimeOffset) =
            match order.Status with
            | InTransit -> 
                let dist = DistanceUtil.calculate partnerLocation order.DeliveryLocation
                if dist < 0.1 then // < 100m
                    Ok { order with Status = Delivered; DeliveredAt = Some now }
                else Error (PreconditionFailed "GeoFence violated: Partner is > 100m away")
            | _ -> Error (InvalidTransition "Can only deliver from InTransit")

        let onCancel (order: Order) (now: DateTimeOffset) =
            let elapsed = (now - order.CreatedAt).TotalSeconds
            match order.Status with
            | Created when elapsed < 60.0 -> Ok { order with Status = OrderStatus.Cancelled; CancelledAt = Some now }
            | Accepted when elapsed < 120.0 -> Ok { order with Status = OrderStatus.Cancelled; CancelledAt = Some now }
            | _ -> Error (TemporalViolation "Cancellation window passed")

        let onReturn (order: Order) (now: DateTimeOffset) =
            match order.Status with
            | Delivered ->
                match order.DeliveredAt with
                | Some d when (now - d).TotalSeconds < 86400.0 -> Ok { order with Status = Returned }
                | _ -> Error (TemporalViolation "Return window passed (24h)")
            | _ -> Error (InvalidTransition "Can only return Delivered orders")

    // Σ2: ONDC Transaction Lifecycle
    module ONDCLifecycle =
        let nextState (currentState: ONDCLifecycleState option) (api: BecknAPI) =
            match currentState, api with
            | None, Search -> Ok Searching
            | Some Searching, Select -> Ok Selected
            | Some Selected, Init -> Ok Initializing
            | Some Initializing, Confirm -> Ok Confirmed
            | Some Confirmed, Status -> Ok Active
            | Some Active, Track -> Ok Active
            | Some Active, Update -> Ok Active
            | Some Searching, Cancel | Some Selected, Cancel | Some Initializing, Cancel | Some Confirmed, Cancel -> Ok ONDCCancelled
            | Some Completed, BecknAPI.Rating -> Ok Completed
            | Some s, Support -> Ok s
            | _ -> Error (InvalidTransition "Invalid ONDC state transition")

    // Σ3: Partner Lifecycle
    module PartnerStateMachine =
        let assignStore (partner: DeliveryPartner) (storeId: Guid) =
            match partner.Status with
            | Online -> Ok { partner with Status = Assigned; AssignedStore = Some storeId }
            | _ -> Error (InvalidTransition "Partner must be Online to assign")

        let addOrder (partner: DeliveryPartner) (orderId: Guid) =
            if partner.ActiveOrders.Length >= (Quantity.value partner.MaxConcurrent) then
                Error (PreconditionFailed "Partner reached MaxConcurrent orders")
            else
                Ok { partner with ActiveOrders = orderId :: partner.ActiveOrders }

    // Σ4: Payment Lifecycle
    module PaymentStateMachine =
        type PaymentContext = { PaymentStatus: PaymentStatus; OrderStatus: OrderStatus; PaymentMethod: PaymentMethod; CashCollected: bool }
        let authorize (ctx: PaymentContext) =
            match ctx.PaymentStatus with
            | Initiated -> Ok { ctx with PaymentStatus = Authorized }
            | _ -> Error (InvalidTransition "Can only authorize Initiated payment")
            
        let capture (ctx: PaymentContext) =
            match ctx.PaymentMethod, ctx.PaymentStatus, ctx.OrderStatus with
            | COD, _, Delivered when ctx.CashCollected -> Ok { ctx with PaymentStatus = Captured }
            | COD, _, _ -> Error (PreconditionFailed "COD must be Delivered and cash collected")
            | _, Authorized, Delivered -> Ok { ctx with PaymentStatus = Captured }
            | _, _, _ -> Error (InvalidTransition "Capture requires Delivered order and Authorized payment")

    // Σ5: Inventory Lifecycle
    module InventoryStateMachine =
        let checkExpiry (item: InventoryItem) (today: DateTimeOffset) =
            match item.ExpiryBatch with
            | Some e when (ExpiryDate.value e) < today.DateTime -> Ok { item with Status = Expired; Available = Quantity.create 0 |> Result.toOption |> Option.get }
            | _ -> Ok item

    // Σ6: Subscription Lifecycle
    module SubscriptionStateMachine =
        let processOrder (sub: Subscription) (now: DateTimeOffset) =
            if sub.Status = SubscriptionStatus.Active && now >= sub.NextDelivery then
                Ok { sub with NextDelivery = now.AddDays(1.0) } // Mock update
            else
                Error (PreconditionFailed "Subscription not active or not due")

module BusinessRules =
    open DomainModel
    open Enums
    open ValueObjects

    // I19: Fare Calculation
    module FareCalculation =
        let calculate (items: OrderItem list) (surgeMultiplier: float) (discountAmt: decimal) =
            let subTotal = items |> List.sumBy (fun i -> (Money.value i.TotalPrice))
            let deliveryFee = 
                if subTotal >= 199.0m then 0.0m
                elif subTotal >= 99.0m then 25.0m
                else 40.0m
            let surgeFee = deliveryFee * decimal (surgeMultiplier - 1.0)
            let totalFare = subTotal + deliveryFee + surgeFee - discountAmt
            
            if totalFare < 0.0m then Error "TotalFare cannot be negative"
            elif totalFare < (subTotal - discountAmt) then Error "TotalFare invalid"
            else Ok (totalFare, deliveryFee, surgeFee)

    // I20: Min Order Value
    let validateMinOrderValue (subTotal: decimal) =
        if subTotal >= 99.0m then Ok ()
        else Error "Minimum order value is ₹99"

    // I21: Store Distance
    let validateStoreDistance (store: DarkStore) (deliveryLoc: GeoCoord) =
        let dist = DistanceUtil.calculate store.Location deliveryLoc
        if dist <= (DeliveryRadius.value store.DeliveryRadius) then Ok ()
        else Error "Delivery address outside store radius"

    // I23: Weight Limit
    let validateWeightLimit (items: OrderItem list) =
        let totalWeight = items |> List.choose (fun i -> i.Weight) |> List.sumBy (fun w -> WeightGrams.value w)
        if totalWeight <= 15000 then Ok ()
        else Error "Weight limit 15kg exceeded"

    // I24: Item Count Limit
    let validateItemCount (items: OrderItem list) =
        if items.Length <= 50 then Ok ()
        else Error "Max 50 items per order"

    // I25: FSSAI Compliance
    let validateFssaiCompliance (store: DarkStore) =
        match store.Status with
        | StoreStatus.Active ->
            match store.FSSAILicense with
            | Some _ -> Ok ()
            | None -> Error "Active store must have FSSAI license"
        | _ -> Ok ()

    // I26: Cold Chain Integrity
    let validateColdChain (order: Order) (partner: DeliveryPartner) (hasInsulatedBag: bool) =
        let hasColdItems = order.Items |> List.exists (fun i -> 
            match i.StorageTemp with 
            | Some t when (TemperatureCelsius.value t) < 5.0 -> true 
            | _ -> false)
        if hasColdItems then
            if (partner.VehicleType = "Bike" || partner.VehicleType = "Scooter") && hasInsulatedBag then Ok ()
            else Error "Cold items require insulated delivery on bike/scooter"
        else Ok ()

    // I28: Printout Size Limit
    let validatePrintoutLimit (printout: PrintoutOrder) =
        let total = (Quantity.value printout.TotalPages) * (Quantity.value printout.Copies)
        if total <= 500 then Ok ()
        else Error "Max 500 pages per printout order"

    // I30: Rating Window
    let validateRatingWindow (order: Order) (now: DateTimeOffset) =
        match order.Status, order.DeliveredAt with
        | Delivered, Some d ->
            if (now - d).TotalSeconds < 172800.0 then Ok ()
            else Error "Rating window passed (48h)"
        | _ -> Error "Order must be Delivered to rate"
