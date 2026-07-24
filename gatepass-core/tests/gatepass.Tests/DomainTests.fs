module DomainTests

open Xunit
open FsCheck
open FsCheck.Xunit
open GatepassCore.Domain
open GatepassCore.Domain.ValueObjects
open GatepassCore.Domain.DomainModel
open GatepassCore.Domain.GatepassBehavior
open System

[<Property>]
let ``PhoneNumber FsAssay creation blocks invalid numbers`` (phoneStr: string) =
    let res = PhoneNumber.create phoneStr
    if isNull phoneStr || phoneStr.Length < 10 || phoneStr.Length > 15 || not (phoneStr |> Seq.forall Char.IsDigit) then
        res = Error PhoneError.TooShort || res = Error PhoneError.TooLong || res = Error PhoneError.ContainsInvalidChars
    else
        match res with
        | Ok p -> PhoneNumber.value p = phoneStr
        | _ -> false

[<Property>]
let ``VehicleRegistration FsAssay blocks invalid formats`` (regStr: string) =
    let res = VehicleRegistration.create regStr
    let regex = System.Text.RegularExpressions.Regex(@"^[A-Z]{2}\s\d{2}\s[A-Z]{1,2}\s\d{4}$")
    if isNull regStr || not (regex.IsMatch(regStr)) then
        res = Error VehicleRegError.InvalidFormat
    else
        match res with
        | Ok p -> VehicleRegistration.value p = regStr
        | _ -> false

[<Property>]
let ``PassDuration FsAssay ensures 1 to 72 hours`` (hours: float) =
    let res = PassDuration.create hours
    if Double.IsNaN(hours) || Double.IsInfinity(hours) then
        res = Error PassDurationError.InvalidValue
    elif hours < 1.0 then
        res = Error PassDurationError.TooShort
    elif hours > 72.0 then
        res = Error PassDurationError.TooLong
    else
        match res with
        | Ok d -> Math.Abs((PassDuration.value d).TotalHours - hours) < 0.0001
        | _ -> false

[<Property>]
let ``Gatepass transitions strictly enforced`` (status: GatepassStatus) =
    let dummyPhone = PhoneNumber.create "1234567890" |> function | Ok p -> p | _ -> failwith "Bad setup"
    let dummyDuration = PassDuration.create 5.0 |> function | Ok d -> d | _ -> failwith "Bad setup"
    let pass = { 
        Id = GatepassId(Guid.NewGuid()); 
        ResidentId = ResidentId(Guid.NewGuid()); 
        VisitorId = VisitorId(Guid.NewGuid()); 
        Phone = dummyPhone; 
        VehicleReg = None; 
        Duration = dummyDuration; 
        Status = status 
    }
    
    // If not Pending, approval should fail
    if status <> Pending then
        match approve pass with
        | Error (InvalidTransition _) -> true
        | _ -> false
    else
        match approve pass with
        | Ok approvedPass -> approvedPass.Status = Approved
        | _ -> false
