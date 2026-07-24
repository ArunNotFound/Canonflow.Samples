namespace UrbanclubCore.Domain

open System
open System.Text.RegularExpressions

// FsAssay Pattern: Primitive Obsession Prevention via Smart Constructors

module ValueObjects =

    type PhoneError = 
        | TooShort
        | TooLong
        
    type PhoneNumber = private PhoneNumber of string
    module PhoneNumber =
        let value (PhoneNumber p) = p
        let create (p: string) =
            if isNull p then Error TooShort
            elif p.Length < 10 then Error TooShort
            elif p.Length > 15 then Error TooLong
            else Ok (PhoneNumber p)

    type NameError =
        | InvalidLength

    type FullName = private FullName of string
    module FullName =
        let value (FullName n) = n
        let create (n: string) =
            if isNull n || n.Length < 2 || n.Length > 100 then Error InvalidLength
            else Ok (FullName n)

    type MoneyError =
        | Negative
        | InvalidNaN

    type MoneyAmount = private MoneyAmount of decimal
    module MoneyAmount =
        let value (MoneyAmount m) = m
        let create (m: decimal) =
            if m < 0.0m then Error Negative
            else Ok (MoneyAmount m)

module DomainModel =
    open ValueObjects

    type UserId = UserId of Guid
    type ServiceId = ServiceId of Guid
    type BookingId = BookingId of Guid

    type UserRole =
        | Customer
        | Professional

    type User = {
        Id: UserId
        Role: UserRole
        FullName: FullName
        Phone: PhoneNumber
    }

    type BookingStatus =
        | Pending
        | Accepted
        | InProgress
        | Completed
        | Cancelled

    type Booking = {
        Id: BookingId
        CustomerId: UserId
        ProfessionalId: UserId option
        ServiceId: ServiceId
        Status: BookingStatus
        ScheduledTime: DateTimeOffset
        TotalAmount: MoneyAmount
        CompletedAt: DateTimeOffset option
    }

    type BookingError =
        | InvalidTransition of BookingStatus * BookingStatus
        | ProfessionalRequired
        | InvalidCompletionTime

module BookingBehavior =
    open DomainModel

    let accept (booking: Booking) (profId: UserId) =
        match booking.Status with
        | Pending -> Ok { booking with Status = Accepted; ProfessionalId = Some profId }
        | _ -> Error (InvalidTransition (booking.Status, Accepted))

    let complete (booking: Booking) (completedTime: DateTimeOffset) =
        match booking.Status with
        | Accepted | InProgress -> 
            if completedTime < booking.ScheduledTime then
                Error InvalidCompletionTime
            else
                Ok { booking with Status = Completed; CompletedAt = Some completedTime }
        | _ -> Error (InvalidTransition (booking.Status, Completed))
