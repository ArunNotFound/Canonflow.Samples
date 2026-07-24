module DomainTests

open Xunit
open FsCheck
open FsCheck.Xunit
open UrbanclubCore.Domain
open UrbanclubCore.Domain.ValueObjects
open UrbanclubCore.Domain.DomainModel
open UrbanclubCore.Domain.BookingBehavior
open System

[<Property>]
let ``MoneyAmount FsAssay creation blocks negative numbers`` (amount: decimal) =
    let res = MoneyAmount.create amount
    if amount < 0.0m then
        res = Error MoneyError.Negative
    else
        match res with
        | Ok m -> MoneyAmount.value m = amount
        | _ -> false

[<Property>]
let ``FullName FsAssay creation blocks invalid lengths`` (nameStr: string) =
    let res = FullName.create nameStr
    if isNull nameStr || nameStr.Length < 2 || nameStr.Length > 100 then
        res = Error NameError.InvalidLength
    else
        match res with
        | Ok n -> FullName.value n = nameStr
        | _ -> false

[<Property>]
let ``Booking cannot be completed before scheduled time`` (completionTime: DateTimeOffset) =
    let dummyMoney = MoneyAmount.create 150.0m |> function | Ok m -> m | _ -> failwith "Bad setup"
    let scheduledTime = DateTimeOffset.Now
    let booking = {
        Id = BookingId(Guid.NewGuid())
        CustomerId = UserId(Guid.NewGuid())
        ProfessionalId = Some (UserId(Guid.NewGuid()))
        ServiceId = ServiceId(Guid.NewGuid())
        Status = Accepted
        ScheduledTime = scheduledTime
        TotalAmount = dummyMoney
        CompletedAt = None
    }

    if completionTime < scheduledTime then
        match complete booking completionTime with
        | Error BookingError.InvalidCompletionTime -> true
        | _ -> false
    else
        match complete booking completionTime with
        | Ok completedBooking -> completedBooking.CompletedAt = Some completionTime
        | _ -> false
