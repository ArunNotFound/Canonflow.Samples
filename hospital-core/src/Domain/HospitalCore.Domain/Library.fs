namespace HospitalCore.Domain

open System

// ==========================================
// 1. Primitive Value Objects & Types (The "Nouns")
// ==========================================
// Instead of simple strings and dates, DDD requires rich types to encapsulate rules.
type PatientId = PatientId of Guid
type DoctorId = DoctorId of Guid
type VisitId = VisitId of Guid
type ClaimId = ClaimId of Guid
type InsuranceId = InsuranceId of Guid

type Gender = Male | Female | Other
type BloodGroup = A_Pos | A_Neg | B_Pos | B_Neg | O_Pos | O_Neg | AB_Pos | AB_Neg

type PatientStatus = Registered | Admitted | Discharged | Deceased
type ClaimStatus = Submitted | UnderReview | Approved | PartialApproved | Rejected

// Example of a refinement type to replace CHECK (date_of_birth <= current_date)
type DateOfBirth = private DateOfBirth of DateTime
module DateOfBirth =
    let create (dob: DateTime) =
        if dob <= DateTime.Today then Ok (DateOfBirth dob)
        else Error "Date of birth cannot be in the future"
    let value (DateOfBirth d) = d

// Example of a refinement type to replace CHECK (duration_days > 0 AND duration_days <= 365)
type PrescriptionDuration = private PrescriptionDuration of int
module PrescriptionDuration =
    let create (days: int) =
        if days > 0 && days <= 365 then Ok (PrescriptionDuration days)
        else Error "Duration must be between 1 and 365 days"
    let value (PrescriptionDuration d) = d

type MoneyAmount = private MoneyAmount of decimal
module MoneyAmount =
    let create (amount: decimal) =
        if amount >= 0m then Ok (MoneyAmount amount)
        else Error "Amount cannot be negative"
    let value (MoneyAmount m) = m


// ==========================================
// 2. Aggregates (The "Nouns" that hold State)
// ==========================================
type Patient = {
    Id: PatientId
    FirstName: string
    LastName: string
    Dob: DateOfBirth
    Gender: Gender
    Status: PatientStatus
}

type Visit = {
    Id: VisitId
    PatientId: PatientId
    DoctorId: DoctorId
    Status: string // Scheduled, InProgress, etc.
}

type Claim = {
    Id: ClaimId
    InsuranceId: InsuranceId
    ClaimedAmount: MoneyAmount
    ApprovedAmount: MoneyAmount option
    Status: ClaimStatus
}

// ==========================================
// 3. Behaviors (The "Verbs" - Missing in the Database Schema)
// ==========================================
// The Database stores the *result* of these verbs (e.g. status='ADMITTED').
// DDD models the *intention* and *execution* of the verbs.

module PatientBehavior =
    type Command =
        | Register of firstName: string * lastName: string * dob: DateTime * gender: Gender
        | Admit
        | Discharge
        | MarkDeceased

    type Event =
        | PatientRegistered of PatientId
        | PatientAdmitted
        | PatientDischarged
        | PatientDeceased

    let execute (cmd: Command) (patient: Patient option) : Result<Event list, string> =
        match cmd, patient with
        | Register (f, l, dob, g), None ->
            match DateOfBirth.create dob with
            | Ok _ -> Ok [ PatientRegistered (PatientId (Guid.NewGuid())) ]
            | Error err -> Error err
            
        | Admit, Some p ->
            match p.Status with
            | Registered | Discharged -> Ok [ PatientAdmitted ]
            | Admitted -> Error "Patient is already admitted."
            | Deceased -> Error "Cannot admit a deceased patient."
            
        | Discharge, Some p ->
            match p.Status with
            | Admitted -> Ok [ PatientDischarged ]
            | _ -> Error "Patient must be admitted to be discharged."
            
        | MarkDeceased, Some p ->
            match p.Status with
            | Deceased -> Error "Patient is already marked deceased."
            | _ -> Ok [ PatientDeceased ]
            
        | _, None -> Error "Patient does not exist."


module ClaimBehavior =
    type Command =
        | SubmitClaim of amount: decimal
        | AdjudicateClaim of approvedAmount: decimal

    type Event =
        | ClaimSubmitted of amount: decimal
        | ClaimAdjudicated of approvedAmount: decimal * isPartial: boolean
        | ClaimRejected of reason: string

    let execute (cmd: Command) (claim: Claim option) : Result<Event list, string> =
        match cmd, claim with
        | SubmitClaim amt, None ->
            match MoneyAmount.create amt with
            | Ok _ -> Ok [ ClaimSubmitted amt ]
            | Error e -> Error e
            
        | AdjudicateClaim approvedAmt, Some c ->
            match c.Status with
            | Submitted | UnderReview ->
                match MoneyAmount.create approvedAmt with
                | Ok validApprovedAmt ->
                    let claimed = MoneyAmount.value c.ClaimedAmount
                    let approved = MoneyAmount.value validApprovedAmt
                    if approved > claimed then
                        Error "Approved amount cannot exceed claimed amount"
                    elif approved = 0m then
                        Ok [ ClaimRejected "Zero payout approved by insurance" ]
                    else
                        let isPartial = approved < claimed
                        Ok [ ClaimAdjudicated (approved, isPartial) ]
                | Error e -> Error e
            | _ -> Error "Claim has already been processed."
            
        | _, None -> Error "Claim does not exist."
