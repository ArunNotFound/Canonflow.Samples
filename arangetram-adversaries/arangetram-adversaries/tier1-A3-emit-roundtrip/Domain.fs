// Adversarial greenfield domain. emit must classify every awkward shape
// honestly on the way out. This is a sketch to expand, not a passing state.
module Adversary.Domain
open System

// recursive — no flat SQL representation
type Employee = { Id: int; Name: string; Manager: Employee option }

// DU with no single-column SQL correlate
type Payment =
  | Cash of decimal
  | Card of last4: string * decimal
  | Upi of vpa: string * decimal
  | Barter of description: string
  | Waived

// option of option — three-valued about three-valued
type Attendance = { Present: bool option option }

// phantom unit — must be Unrepresentable in SQL
type [<Measure>] rupee
type Fee = { Amount: decimal<rupee> }

// OR-predicate refined — emit as CHECK(...OR...) or classify Approximate
// (pseudocode; wire to real Refined once emit exists)
// type EligibleAge = Refined<int, (fun a -> a >= 21 || a = 0)>
