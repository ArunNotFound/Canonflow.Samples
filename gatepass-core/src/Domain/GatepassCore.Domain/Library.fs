namespace GatepassCore.Domain

open System
open System.Text.RegularExpressions

// FsAssay Pattern: Primitive Obsession Prevention via Smart Constructors

module ValueObjects =

    type PhoneError = 
        | TooShort
        | TooLong
        | ContainsInvalidChars

    type PhoneNumber = private PhoneNumber of string
    module PhoneNumber =
        let value (PhoneNumber p) = p
        let create (p: string) =
            if isNull p then Error TooShort
            elif p.Length < 10 then Error TooShort
            elif p.Length > 15 then Error TooLong
            elif not (p |> Seq.forall Char.IsDigit) then Error ContainsInvalidChars
            else Ok (PhoneNumber p)

    type VehicleRegError =
        | InvalidFormat
        
    type VehicleRegistration = private VehicleRegistration of string
    module VehicleRegistration =
        let value (VehicleRegistration v) = v
        let create (v: string) =
            // Indian standard pattern: KA 01 AB 1234
            let regex = Regex(@"^[A-Z]{2}\s\d{2}\s[A-Z]{1,2}\s\d{4}$")
            if isNull v || not (regex.IsMatch(v)) then Error InvalidFormat
            else Ok (VehicleRegistration v)

    type PassDurationError =
        | InvalidValue
        | TooShort
        | TooLong

    type PassDuration = private PassDuration of TimeSpan
    module PassDuration =
        let value (PassDuration d) = d
        let create (hours: float) =
            if Double.IsNaN(hours) || Double.IsInfinity(hours) then Error InvalidValue
            elif hours < 1.0 then Error TooShort
            elif hours > 72.0 then Error TooLong
            else Ok (PassDuration (TimeSpan.FromHours(hours)))

module DomainModel =
    open ValueObjects

    type ResidentId = ResidentId of Guid
    type VisitorId = VisitorId of Guid
    type GatepassId = GatepassId of Guid

    type GatepassStatus =
        | Pending
        | Approved
        | Declined
        | Entered
        | Exited
        | Expired

    type Gatepass = {
        Id: GatepassId
        ResidentId: ResidentId
        VisitorId: VisitorId
        Phone: PhoneNumber
        VehicleReg: VehicleRegistration option
        Duration: PassDuration
        Status: GatepassStatus
    }

    type GatepassError =
        | InvalidTransition of GatepassStatus * GatepassStatus

module GatepassBehavior =
    open DomainModel

    let approve (pass: Gatepass) =
        match pass.Status with
        | Pending -> Ok { pass with Status = Approved }
        | _ -> Error (InvalidTransition (pass.Status, Approved))
        
    let decline (pass: Gatepass) =
        match pass.Status with
        | Pending -> Ok { pass with Status = Declined }
        | _ -> Error (InvalidTransition (pass.Status, Declined))
        
    let markEntered (pass: Gatepass) =
        match pass.Status with
        | Approved -> Ok { pass with Status = Entered }
        | _ -> Error (InvalidTransition (pass.Status, Entered))
        
    let markExited (pass: Gatepass) =
        match pass.Status with
        | Entered -> Ok { pass with Status = Exited }
        | _ -> Error (InvalidTransition (pass.Status, Exited))
        
    let markExpired (pass: Gatepass) =
        // Only Pending or Approved passes can expire (e.g. if the visitor never shows up)
        match pass.Status with
        | Pending | Approved -> Ok { pass with Status = Expired }
        | _ -> Error (InvalidTransition (pass.Status, Expired))
