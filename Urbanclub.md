# $\mathcal{U}$RBAN $\mathcal{C}$LAN — Formal Requirements Specification

## Consumer App Core $\times$ Partner App

**Version:** $\nu_0$ · **Date:** 25 July 2026 · **Status:** Axiomatic

---

## $\S 0$. Notation & Conventions

| Symbol | Meaning |
|---|---|
| $\mathcal{D}$ | Domain (functional core) |
| $\mathcal{S}$ | Shell (imperative boundary) |
| $\mathcal{F}$ | FsAssay rule set |
| $\mathcal{C}$ | CanonFlow pipeline |
| $\text{Ok}(x)$ | Successful construction |
| $\text{Err}(e)$ | Rejected construction |
| $\Sigma$ | State machine |
| $\delta$ | Transition function |
| $\mathcal{I}$ | Invariant set |
| $\forall$ | For all |
| $\exists$ | There exists |
| $\nexists$ | There does not exist |
| $\in$ | Element of |
| $\notin$ | Not element of |
| $\rightarrow$ | Function / implies |
| $\times$ | Cartesian product |
| $\cup$ | Union |
| $\cap$ | Intersection |
| $\emptyset$ | Empty set |
| $\bot$ | Undefined / crash |
| $\top$ | Valid / success |
| $\mathbb{N}$ | Natural numbers |
| $\mathbb{R}$ | Real numbers |
| $\mathbb{Z}$ | Integers |
| $\lambda$ | Lambda abstraction |
| $\equiv$ | Equivalent |
| $\triangleq$ | Defined as |
| $\vdash$ | Proves / entails |
| $\models$ | Satisfies |
| $\square$ | Always (temporal) |
| $\diamond$ | Eventually (temporal) |
| $\blacksquare$ | Q.E.D. |

---

## $\S 1$. Architectural Axioms (FCIS)

### Axiom 1.1 — Functional Core Purity

$$\forall f \in \mathcal{D},\ \forall x \in \text{dom}(f):\quad f(x) \equiv f(x)$$

*The core is deterministic. Same input, same output. Always.*

### Axiom 1.2 — Shell Isolation

$$\mathcal{D} \cap \mathcal{S} = \emptyset$$

*The core and shell share no mutable state. The shell calls the core. The core never calls the shell.*

### Axiom 1.3 — No Side Effects in Core

$$\forall f \in \mathcal{D}:\quad \text{IO}(f) = \emptyset \;\wedge\; \text{Mut}(f) = \emptyset \;\wedge\; \text{Exn}(f) = \emptyset$$

*No I/O. No mutation. No exceptions. In the core.*

### Axiom 1.4 — FsAssay Enforcement

$$\forall f \in \mathcal{D}:\quad \mathcal{F}(f) = \emptyset$$

*Every function in the core passes all FsAssay rules. Zero violations.*

### Axiom 1.5 — CanonFlow Fidelity

$$\forall c \in \text{Constraints}(\text{DB}):\quad \text{CanonFlow}(c) \models c_{\text{TS}} \;\vee\; \text{Guard}(c) \in \mathcal{S}$$

*Every DB constraint is either translated to TypeScript OR guarded by backend middleware. No silent gaps.*

---

## $\S 2$. Primitive Type Definitions (Smart Constructors)

### Definition 2.1 — PhoneNumber

$$\text{PhoneNumber} \triangleq \{ p \in \text{String} \mid 10 \leq |p| \leq 15 \;\wedge\; \forall c \in p:\ c \in \{0,1,...,9\} \}$$

**Smart Constructor:**

$$\text{PhoneNumber.create} : \text{String} \rightarrow \text{Result}(\text{PhoneNumber},\ \text{PhoneError})$$

$$\text{PhoneError} \triangleq \text{TooShort} \mid \text{TooLong} \mid \text{ContainsInvalidChars} \mid \text{Null}$$

**FsAssay Rule:** $\text{FSA1004}$ (Primitive Obsession) — `string` $\rightarrow$ `PhoneNumber`

**DB Constraint:**

```sql
CONSTRAINT chk_phone CHECK (length(phone) >= 10 AND length(phone) <= 15
                            AND phone ~ '^[0-9]+$')
```

**Invariant:**

$$\forall p \in \text{PhoneNumber}:\quad 10 \leq |p| \leq 15 \;\wedge\; p \in [0\text{-}9]^*$$

---

### Definition 2.2 — Email

$$\text{Email} \triangleq \{ e \in \text{String} \mid e \neq \emptyset \;\wedge\; \exists!\, i:\ e_i = \text{@} \;\wedge\; \exists\, j > i:\ e_j = \text{.} \;\wedge\; \text{ws}(e) = \emptyset \}$$

**Smart Constructor:**

$$\text{Email.create} : \text{String} \rightarrow \text{Result}(\text{Email},\ \text{EmailError})$$

$$\text{EmailError} \triangleq \text{Empty} \mid \text{MissingAt} \mid \text{MultipleAt} \mid \text{MissingDot} \mid \text{ContainsWhitespace} \mid \text{InvalidFormat}$$

**FsAssay Rule:** $\text{FSA1004}$

**DB Constraint:**

```sql
CONSTRAINT chk_email CHECK (email ~ '^[^@\s]+@[^@\s]+\.[^@\s]+$')
```

**Invariant:**

$$\forall e \in \text{Email}:\quad |\{i : e_i = \text{@}\}| = 1 \;\wedge\; \exists\, j > \text{pos}(\text{@}):\ e_j = \text{.}$$

---

### Definition 2.3 — VehicleRegistration (Partner App)

$$\text{VehicleReg} \triangleq \{ v \in \text{String} \mid v \sim \texttt{[A-Z]\{2\}\textbackslash s[0-9]\{2\}\textbackslash s[A-Z]\{1,2\}\textbackslash s[0-9]\{4\}} \}$$

*Indian standard: KA 01 AB 1234*

**Smart Constructor:**

$$\text{VehicleReg.create} : \text{String} \rightarrow \text{Result}(\text{VehicleReg},\ \text{VehicleRegError})$$

$$\text{VehicleRegError} \triangleq \text{InvalidFormat} \mid \text{Null}$$

**FsAssay Rule:** $\text{FSA1004}$

---

### Definition 2.4 — Money

$$\text{Money} \triangleq \{ m \in \mathbb{R} \mid m \geq 0 \;\wedge\; \neg\text{NaN}(m) \;\wedge\; \neg\text{Inf}(m) \;\wedge\; \text{precision}(m) \leq 2 \}$$

**Smart Constructor:**

$$\text{Money.create} : \text{decimal} \rightarrow \text{Result}(\text{Money},\ \text{MoneyError})$$

$$\text{MoneyError} \triangleq \text{Negative} \mid \text{NaN} \mid \text{Infinity} \mid \text{ExceedsPrecision}$$

**FsAssay Rule:** $\text{FSA1004}$

**Invariant:**

$$\forall m \in \text{Money}:\quad m \geq 0 \;\wedge\; m \in \mathbb{Q} \;\wedge\; \text{denominator}(m) \mid 100$$

---

### Definition 2.5 — Distance (km)

$$\text{Distance} \triangleq \{ d \in \mathbb{R} \mid 0 < d \leq 100 \;\wedge\; \neg\text{NaN}(d) \;\wedge\; \neg\text{Inf}(d) \}$$

**Smart Constructor:**

$$\text{Distance.create} : \text{float} \rightarrow \text{Result}(\text{Distance},\ \text{DistanceError})$$

$$\text{DistanceError} \triangleq \text{NonPositive} \mid \text{ExceedsMax} \mid \text{NaN} \mid \text{Infinity}$$

---

### Definition 2.6 — Duration (minutes)

$$\text{Duration} \triangleq \{ d \in \mathbb{N} \mid 1 \leq d \leq 1440 \}$$

*1 minute to 24 hours.*

**Smart Constructor:**

$$\text{Duration.create} : \text{int} \rightarrow \text{Result}(\text{Duration},\ \text{DurationError})$$

$$\text{DurationError} \triangleq \text{TooShort} \mid \text{TooLong} \mid \text{Negative}$$

---

### Definition 2.7 — GeoCoordinate

$$\text{GeoCoord} \triangleq \{ (\text{lat}, \text{lng}) \in \mathbb{R}^2 \mid -90 \leq \text{lat} \leq 90 \;\wedge\; -180 \leq \text{lng} \leq 180 \}$$

**Smart Constructor:**

$$\text{GeoCoord.create} : \text{float} \times \text{float} \rightarrow \text{Result}(\text{GeoCoord},\ \text{GeoError})$$

$$\text{GeoError} \triangleq \text{InvalidLatitude} \mid \text{InvalidLongitude} \mid \text{NaN}$$

---

### Definition 2.8 — Rating

$$\text{Rating} \triangleq \{ r \in \mathbb{R} \mid 1.0 \leq r \leq 5.0 \;\wedge\; \text{precision}(r) \leq 1 \}$$

**Smart Constructor:**

$$\text{Rating.create} : \text{float} \rightarrow \text{Result}(\text{Rating},\ \text{RatingError})$$

$$\text{RatingError} \triangleq \text{BelowMin} \mid \text{AboveMax} \mid \text{NaN}$$

---

### Definition 2.9 — OTP

$$\text{OTP} \triangleq \{ o \in [0\text{-}9]^6 \}$$

*Exactly 6 digits.*

**Smart Constructor:**

$$\text{OTP.create} : \text{String} \rightarrow \text{Result}(\text{OTP},\ \text{OTPError})$$

$$\text{OTPError} \triangleq \text{WrongLength} \mid \text{ContainsNonDigit} \mid \text{Expired}$$

**Invariant:**

$$\forall o \in \text{OTP}:\quad |o| = 6 \;\wedge\; o \in [0\text{-}9]^6$$

---

### Definition 2.10 — IFSCCode (Payment)

$$\text{IFSC} \triangleq \{ c \in \text{String} \mid c \sim \texttt{[A-Z]\{4\}0[A-Z0-9]\{6\}} \}$$

*Indian banking: SBIN0001234*

---

### Definition 2.11 — PANNumber (Partner KYC)

$$\text{PAN} \triangleq \{ p \in \text{String} \mid p \sim \texttt{[A-Z]\{5\}[0-9]\{4\}[A-Z]\} \}$$

*ABCDE1234F*

---

### Definition 2.12 — Pincode

$$\text{Pincode} \triangleq \{ p \in [0\text{-}9]^6 \mid p_1 \neq 0 \}$$

*6 digits, first digit non-zero.*

---

## $\S 3$. Domain Entities

### Definition 3.1 — Consumer

$$\text{Consumer} \triangleq \left\{ \begin{array}{l} \text{Id} : \text{Guid} \\ \text{Phone} : \text{PhoneNumber} \\ \text{Email} : \text{Email} \\ \text{Name} : \text{NonEmptyString} \\ \text{Addresses} : \text{Address}^+ \\ \text{DefaultAddress} : \text{Address} \\ \text{Wallet} : \text{Money} \\ \text{Rating} : \text{Rating} \\ \text{Status} : \text{ConsumerStatus} \\ \text{CreatedAt} : \text{Instant} \end{array} \right\}$$

### Definition 3.2 — Partner

$$\text{Partner} \triangleq \left\{ \begin{array}{l} \text{Id} : \text{Guid} \\ \text{Phone} : \text{PhoneNumber} \\ \text{Email} : \text{Email} \\ \text{Name} : \text{NonEmptyString} \\ \text{PAN} : \text{PAN} \\ \text{VehicleReg} : \text{VehicleReg} \\ \text{VehicleType} : \text{VehicleType} \\ \text{ServiceTypes} : \text{ServiceType}^+ \\ \text{CurrentLocation} : \text{GeoCoord} \\ \text{Status} : \text{PartnerStatus} \\ \text{Rating} : \text{Rating} \\ \text{Earnings} : \text{Money} \\ \text{KYC} : \text{KYCStatus} \\ \text{BankAccount} : \text{BankAccount} \\ \text{CreatedAt} : \text{Instant} \end{array} \right\}$$

### Definition 3.3 — Order

$$\text{Order} \triangleq \left\{ \begin{array}{l} \text{Id} : \text{Guid} \\ \text{ConsumerId} : \text{Guid} \\ \text{PartnerId} : \text{Guid option} \\ \text{ServiceType} : \text{ServiceType} \\ \text{Pickup} : \text{GeoCoord} \times \text{Address} \\ \text{Dropoff} : \text{GeoCoord} \times \text{Address} \\ \text{Distance} : \text{Distance} \\ \text{EstimatedDuration} : \text{Duration} \\ \text{BaseFare} : \text{Money} \\ \text{SurgeMultiplier} : \text{SurgeMultiplier} \\ \text{TotalFare} : \text{Money} \\ \text{PaymentMethod} : \text{PaymentMethod} \\ \text{PaymentStatus} : \text{PaymentStatus} \\ \text{Status} : \text{OrderStatus} \\ \text{OTP} : \text{OTP} \\ \text{CreatedAt} : \text{Instant} \\ \text{ConfirmedAt} : \text{Instant option} \\ \text{PickedUpAt} : \text{Instant option} \\ \text{DeliveredAt} : \text{Instant option} \\ \text{CancelledAt} : \text{Instant option} \\ \text{CancellationReason} : \text{CancellationReason option} \end{array} \right\}$$

### Definition 3.4 — ServiceType (DU)

$$\text{ServiceType} \triangleq \text{Grocery} \mid \text{Food} \mid \text{Parcel} \mid \text{Ride} \mid \text{HomeService} \mid \text{Pharmacy}$$

**FsAssay Rule:** $\text{FSA1004}$ — Not a `string`. A DU. Compiler-enforced exhaustiveness.

### Definition 3.5 — VehicleType (DU)

$$\text{VehicleType} \triangleq \text{Bicycle} \mid \text{Bike} \mid \text{Scooter} \mid \text{Car} \mid \text{Auto} \mid \text{Truck}$$

### Definition 3.6 — PaymentMethod (DU)

$$\text{PaymentMethod} \triangleq \text{UPI} \mid \text{Card} \mid \text{Wallet} \mid \text{Cash} \mid \text{NetBanking}$$

### Definition 3.7 — SurgeMultiplier

$$\text{SurgeMultiplier} \triangleq \{ s \in \mathbb{R} \mid 1.0 \leq s \leq 5.0 \;\wedge\; \text{precision}(s) \leq 1 \}$$

**Smart Constructor:**

$$\text{Surge.create} : \text{float} \rightarrow \text{Result}(\text{SurgeMultiplier},\ \text{SurgeError})$$

$$\text{SurgeError} \triangleq \text{BelowMin} \mid \text{AboveMax} \mid \text{NaN}$$

---

## $\S 4$. State Machines

### $\Sigma_1$: Order Lifecycle

$$\Sigma_1 = (Q_1,\ \delta_1,\ q_0^1,\ F_1)$$

**States:**

$$Q_1 = \{\text{Placed},\ \text{Confirmed},\ \text{PartnerAssigned},\ \text{PickedUp},\ \text{InTransit},\ \text{Delivered},\ \text{Cancelled},\ \text{Refunded}\}$$

**Initial State:**

$$q_0^1 = \text{Placed}$$

**Terminal States:**

$$F_1 = \{\text{Delivered},\ \text{Refunded}\}$$

**Transition Function $\delta_1$:**

$$\delta_1 : Q_1 \times \text{Event} \rightarrow \text{Result}(Q_1,\ \text{TransitionError})$$

| From | Event | To | Guard |
|---|---|---|---|
| Placed | Confirm | Confirmed | $\text{PaymentStatus} = \text{Authorized}$ |
| Placed | Cancel | Cancelled | $\text{CancellationReason} \neq \emptyset$ |
| Confirmed | AssignPartner | PartnerAssigned | $\text{PartnerId} \neq \emptyset$ |
| Confirmed | Cancel | Cancelled | $t_{\text{now}} - t_{\text{created}} < 120\text{s}$ |
| PartnerAssigned | Pickup | PickedUp | $\text{OTP verified}$ |
| PartnerAssigned | Cancel | Cancelled | $\text{Partner consent}$ |
| PickedUp | StartTransit | InTransit | — |
| InTransit | Deliver | Delivered | $\text{OTP verified} \;\wedge\; \text{GeoCoord} \approx \text{Dropoff}$ |
| Cancelled | Refund | Refunded | $\text{PaymentStatus} = \text{Captured}$ |

**Invariant $\mathcal{I}_1$ (Temporal Ordering):**

$$\square\left( t_{\text{created}} \leq t_{\text{confirmed}} \leq t_{\text{assigned}} \leq t_{\text{picked}} \leq t_{\text{transit}} \leq t_{\text{delivered}} \right)$$

**Invariant $\mathcal{I}_2$ (Terminal Finality):**

$$\forall q \in F_1:\quad \nexists\, e \in \text{Event}:\ \delta_1(q, e) \neq \text{Err}(\text{TerminalState})$$

*Once Delivered or Refunded, no transition is possible.*

**Invariant $\mathcal{I}_3$ (Cancellation Window):**

$$\delta_1(\text{Confirmed}, \text{Cancel}) = \text{Ok}(\text{Cancelled}) \iff t_{\text{now}} - t_{\text{created}} < 120\text{s}$$

*Free cancellation only within 2 minutes.*

**Invariant $\mathcal{I}_4$ (OTP Gate):**

$$\delta_1(\text{PartnerAssigned}, \text{Pickup}) = \text{Ok}(\text{PickedUp}) \iff \text{OTP}_{\text{input}} = \text{OTP}_{\text{order}}$$

*Partner cannot pick up without consumer's OTP.*

**Invariant $\mathcal{I}_5$ (Geo-Fence at Delivery):**

$$\delta_1(\text{InTransit}, \text{Deliver}) = \text{Ok}(\text{Delivered}) \iff d(\text{Partner}_{\text{loc}},\ \text{Dropoff}_{\text{coord}}) < 200\text{m}$$

*Partner must be within 200m of dropoff to complete delivery.*

**FsAssay Enforcement:**

| Rule | Application |
|---|---|
| FSA1004 | `OrderStatus` is DU, not `string` |
| FSA1006 | All transitions return `Result`, no `failwith` |
| FSA-AI10 | No magic strings `"PLACED"`, `"CONFIRMED"` |
| FSA2022 | No I/O in transition functions |

---

### $\Sigma_2$: Partner Lifecycle

$$\Sigma_2 = (Q_2,\ \delta_2,\ q_0^2,\ F_2)$$

**States:**

$$Q_2 = \{\text{Registered},\ \text{KYCPending},\ \text{KYCVerified},\ \text{Online},\ \text{OnTrip},\ \text{Offline},\ \text{Suspended},\ \text{Deactivated}\}$$

**Transition Function $\delta_2$:**

| From | Event | To | Guard |
|---|---|---|---|
| Registered | SubmitKYC | KYCPending | $\text{PAN} \neq \emptyset \;\wedge\; \text{VehicleReg} \neq \emptyset$ |
| KYCPending | ApproveKYC | KYCVerified | $\text{Admin review}$ |
| KYCPending | RejectKYC | Registered | $\text{Reason provided}$ |
| KYCVerified | GoOnline | Online | $\text{VehicleReg valid} \;\wedge\; \text{Insurance valid}$ |
| Online | AcceptOrder | OnTrip | $\text{Order} \neq \emptyset$ |
| OnTrip | CompleteOrder | Online | $\text{Order.Status} = \text{Delivered}$ |
| Online | GoOffline | Offline | — |
| Offline | GoOnline | Online | — |
| Online | Suspend | Suspended | $\text{Admin action} \;\vee\; \text{Rating} < 3.0$ |
| Suspended | Reactivate | Online | $\text{Admin action}$ |
| Suspended | Deactivate | Deactivated | $\text{Admin action}$ |

**Invariant $\mathcal{I}_6$ (KYC Gate):**

$$\forall p \in \text{Partner}:\quad p.\text{Status} \in \{\text{Online}, \text{OnTrip}\} \implies p.\text{KYC} = \text{Verified}$$

*No partner can go online without verified KYC.*

**Invariant $\mathcal{I}_7$ (Single Active Trip):**

$$\forall p \in \text{Partner}:\quad p.\text{Status} = \text{OnTrip} \implies |\{o \in \text{Order} : o.\text{PartnerId} = p.\text{Id} \;\wedge\; o.\text{Status} \notin F_1\}| = 1$$

*A partner can have exactly one active order at a time.*

**Invariant $\mathcal{I}_8$ (Rating Suspension):**

$$\forall p \in \text{Partner}:\quad p.\text{Rating} < 3.0 \implies \diamond(p.\text{Status} = \text{Suspended})$$

*Partners below 3.0 rating are eventually suspended.*

---

### $\Sigma_3$: Payment Lifecycle

$$\Sigma_3 = (Q_3,\ \delta_3,\ q_0^3,\ F_3)$$

**States:**

$$Q_3 = \{\text{Initiated},\ \text{Authorized},\ \text{Captured},\ \text{Settled},\ \text{Failed},\ \text{RefundInitiated},\ \text{Refunded}\}$$

**Transition Function $\delta_3$:**

| From | Event | To | Guard |
|---|---|---|---|
| Initiated | Authorize | Authorized | $\text{Payment gateway response} = \text{OK}$ |
| Initiated | Fail | Failed | $\text{Gateway error}$ |
| Authorized | Capture | Captured | $\text{Order.Status} = \text{Delivered}$ |
| Authorized | Void | Failed | $\text{Order.Status} = \text{Cancelled}$ |
| Captured | Settle | Settled | $\text{T+2 settlement}$ |
| Captured | InitiateRefund | RefundInitiated | $\text{Order.Status} = \text{Cancelled}$ |
| RefundInitiated | CompleteRefund | Refunded | $\text{Gateway confirmation}$ |

**Invariant $\mathcal{I}_9$ (Capture Gate):**

$$\delta_3(\text{Authorized}, \text{Capture}) = \text{Ok}(\text{Captured}) \iff \text{Order.Status} = \text{Delivered}$$

*Payment is captured ONLY after delivery. Never before.*

**Invariant $\mathcal{I}_{10}$ (Refund Bounds):**

$$\forall r \in \text{Refund}:\quad 0 < r.\text{Amount} \leq \text{Order.TotalFare}$$

*Refund cannot exceed the original fare.*

**Invariant $\mathcal{I}_{11}$ (No Double Capture):**

$$\forall o \in \text{Order}:\quad |\{p \in \text{Payment} : p.\text{OrderId} = o.\text{Id} \;\wedge\; p.\text{Status} = \text{Captured}\}| \leq 1$$

*An order can be captured at most once.*

---

### $\Sigma_4$: Consumer Lifecycle

$$Q_4 = \{\text{Registered},\ \text{Verified},\ \text{Active},\ \text{Suspended},\ \text{Deactivated}\}$$

| From | Event | To | Guard |
|---|---|---|---|
| Registered | VerifyOTP | Verified | $\text{OTP valid}$ |
| Verified | FirstOrder | Active | — |
| Active | Suspend | Suspended | $\text{Fraud detected} \;\vee\; \text{3 failed payments}$ |
| Suspended | Reactivate | Active | $\text{Admin action}$ |
| Suspended | Deactivate | Deactivated | $\text{Admin action}$ |

---

## $\S 5$. Business Rule Invariants

### $\mathcal{I}_{12}$: Fare Calculation

$$\text{TotalFare} = \text{BaseFare} \times \text{SurgeMultiplier} + \text{DistanceFare} + \text{PlatformFee} - \text{Discount}$$

where:

$$\text{DistanceFare} = \begin{cases} 0 & \text{if } d \leq d_{\text{free}} \\ r_{\text{km}} \times (d - d_{\text{free}}) & \text{if } d > d_{\text{free}} \end{cases}$$

**Invariant:**

$$\forall o \in \text{Order}:\quad o.\text{TotalFare} \geq 0 \;\wedge\; o.\text{TotalFare} \geq o.\text{BaseFare}$$

*Total fare is never negative. Never less than base fare.*

### $\mathcal{I}_{13}$: Partner Earnings

$$\text{PartnerEarnings} = \text{TotalFare} - \text{PlatformCommission}$$

$$\text{PlatformCommission} = \text{TotalFare} \times r_{\text{commission}}$$

where $r_{\text{commission}} \in [0.10, 0.30]$

**Invariant:**

$$\forall o \in \text{Order}:\quad o.\text{PartnerEarnings} > 0 \;\wedge\; o.\text{PartnerEarnings} < o.\text{TotalFare}$$

*Partner always earns something. Platform always takes a cut.*

### $\mathcal{I}_{14}$: Surge Bounds

$$\forall s \in \text{SurgeMultiplier}:\quad 1.0 \leq s \leq 5.0$$

*Surge never exceeds 5×. Never below 1×.*

### $\mathcal{I}_{15}$: Cancellation Fee

$$\text{CancellationFee} = \begin{cases} 0 & \text{if } t_{\text{cancel}} - t_{\text{created}} < 120\text{s} \\ 25 & \text{if } 120\text{s} \leq t_{\text{cancel}} - t_{\text{created}} < 300\text{s} \\ 50 & \text{if } t_{\text{cancel}} - t_{\text{created}} \geq 300\text{s} \;\wedge\; \text{Status} \notin \{\text{PickedUp}, \text{InTransit}\} \\ \text{TotalFare} & \text{if } \text{Status} \in \{\text{PickedUp}, \text{InTransit}\} \end{cases}$$

### $\mathcal{I}_{16}$: Wallet Balance

$$\forall c \in \text{Consumer}:\quad c.\text{Wallet} \geq 0$$

*Wallet balance is never negative.*

### $\mathcal{I}_{17}$: Service Radius

$$\forall o \in \text{Order}:\quad d(o.\text{Pickup},\ o.\text{Dropoff}) \leq 50\text{km}$$

*Maximum service distance is 50km.*

### $\mathcal{I}_{18}$: Partner-Consumer Distance at Assignment

$$\forall o \in \text{Order}:\quad o.\text{Status} = \text{PartnerAssigned} \implies d(\text{Partner}_{\text{loc}},\ o.\text{Pickup}) \leq 5\text{km}$$

*Assigned partner must be within 5km of pickup.*

### $\mathcal{I}_{19}$: OTP Expiry

$$\forall o \in \text{Order}:\quad \text{OTP valid} \iff t_{\text{now}} - t_{\text{created}} < 600\text{s}$$

*OTP expires after 10 minutes.*

### $\mathcal{I}_{20}$: Rating Submission Window

$$\forall o \in \text{Order}:\quad \text{Rating allowed} \iff o.\text{Status} = \text{Delivered} \;\wedge\; t_{\text{now}} - t_{\text{delivered}} < 86400\text{s}$$

*Rating can be submitted within 24 hours of delivery.*

---

## $\S 6$. FsAssay Rule Mapping

| FsAssay Rule | Domain Application | Enforcement |
|---|---|---|
| **FSA1001** (Mutable) | No `mutable` in fare calculation, state transitions, validation | TAST: `binding.IsMutable` |
| **FSA1002** (Option.get) | All optional fields (`PartnerId`, `CancelledAt`) matched explicitly | TAST: `FSharpExprPatterns.Call` |
| **FSA1003** (Null/defaultof) | No null in domain. `Option` for absence. | TAST: `DefaultValue` |
| **FSA1004** (Primitive Obsession) | 12 smart constructors replace all primitives | Regex on sanitized source |
| **FSA1005** (Boolean Validation) | `isValid` predicates replaced with `Result` returns | Regex |
| **FSA1006** (Exception Flow) | All transitions return `Result<'T, 'Error>` | TAST |
| **FSA1007** (Imperative Loops) | Fare calculation uses `List.fold`, not `while` | Regex |
| **FSA1008** (Inheritance) | No OOP inheritance. DUs + modules only. | Regex (excluding `inherit exn`) |
| **FSA1009** (God Objects) | No module > 200 lines. Split by bounded context. | Regex |
| **FSA2008** (Async.RunSynchronously) | Only at `[<EntryPoint>]` | TAST |
| **FSA2012** (printfn in library) | No console output in domain. Logging via shell. | TAST |
| **FSA2016** (Dependency Chain) | Max 4 layers: API → Service → Domain → DB | Architectural |
| **FSA2017** (Circular Dependency) | No circular module references | Architectural |
| **FSA2022** (Impure Core) | No `System.IO`, `HttpClient`, `Console` in domain | TAST |
| **FSA-SEC01** (Hard-coded Secrets) | No API keys, passwords, connection strings in code | Regex |
| **FSA-SEC02** (SQL Injection) | No `sprintf` SQL. Parameterized queries only. | Regex |
| **FSA-SEC04** (Weak Crypto) | No MD5/SHA1. Use SHA256+ for hashing. | Regex |
| **FSA-SEC05** (Disabled SSL) | No `ServerCertificateValidationCallback = true` | Regex |
| **FSA-AI01** (Dead Code) | No unused functions in domain | Regex (file-local) |
| **FSA-AI02** (Duplicate Code) | No copy-paste blocks > 6 lines | Regex |
| **FSA-AI04** (Commented-out Code) | No `// let ...` blocks | Regex on raw source |
| **FSA-AI05** (Inconsistent Error Handling) | One error strategy per module: `Result` everywhere | Regex |
| **FSA-AI10** (Magic Numbers) | All constants named: `surgeMax`, `otpExpirySeconds`, `maxDistanceKm` | Regex |

---

## $\S 7$. CanonFlow Pipeline Requirements

### $\mathcal{C}_1$: DB → F# Fidelity

$$\forall c \in \text{Constraints}(\text{DB}):\quad \exists\, sc \in \text{SmartConstructors}(\mathcal{D}):\quad sc \models c$$

*Every DB CHECK constraint has a corresponding F# smart constructor.*

### $\mathcal{C}_2$: F# → TypeScript Fidelity

$$\forall sc \in \text{SmartConstructors}(\mathcal{D}):\quad \text{CanonFlow}(sc) \rightarrow \text{ZodValidator}(\text{TS})$$

*Every smart constructor generates a Zod validator.*

### $\mathcal{C}_3$: F# → OpenAPI Fidelity

$$\forall t \in \text{Types}(\mathcal{D}):\quad \text{CanonFlow}(t) \rightarrow \text{OpenAPISchema}$$

*Every domain type generates an OpenAPI schema.*

### $\mathcal{C}_4$: FsCheck Generator Derivation

$$\forall c \in \text{Constraints}(\text{DB}):\quad \text{CanonFlow}(c) \rightarrow \text{FsCheck.Arbitrary}$$

*Every DB constraint generates an FsCheck arbitrary that respects the constraint.*

### $\mathcal{C}_5$: PROOF.md Generation

$$\text{CanonFlow}(\text{DB}) \rightarrow \text{PROOF.md} : \{c \mapsto (\text{Exact} \mid \text{Unsupported})\}$$

*Every constraint is classified as exactly translated or requiring manual guard.*

### $\mathcal{C}_6$: Layer Map Generation

$$\text{CanonFlow}(\text{Architecture}) \rightarrow \text{layer-map.md} : \{\text{layer} \mapsto \text{responsibilities}\}$$

---

## $\S 8$. Security Requirements

### $\mathcal{S}_1$: Authentication

$$\forall \text{request} \in \text{API}:\quad \text{Authenticated}(\text{request}) \implies \exists\, \text{JWT}:\ \text{Valid}(\text{JWT}) \;\wedge\; \neg\text{Expired}(\text{JWT})$$

### $\mathcal{S}_2$: Authorization

$$\forall \text{request} \in \text{API}:\quad \text{Authorized}(\text{request}) \implies \text{Role}(\text{request}) \supseteq \text{RequiredRole}(\text{endpoint})$$

**Roles:**

$$\text{Role} \triangleq \text{Consumer} \mid \text{Partner} \mid \text{Admin} \mid \text{Support}$$

### $\mathcal{S}_3$: OTP Security

$$\forall o \in \text{OTP}:\quad \text{Attempts}(o) \leq 3 \implies \text{Valid}(o)$$

$$\text{Attempts}(o) > 3 \implies \text{Locked}(o) \;\wedge\; \text{LockDuration} = 300\text{s}$$

*3 failed OTP attempts → 5 minute lockout.*

### $\mathcal{S}_4$: Rate Limiting

$$\forall u \in \text{User}:\quad |\{\text{req} \in \text{Requests} : \text{req.user} = u \;\wedge\; t_{\text{req}} \in [t_{\text{now}} - 60\text{s},\ t_{\text{now}}]\}| \leq 60$$

*Max 60 requests per minute per user.*

### $\mathcal{S}_5$: Data Encryption

$$\forall d \in \text{PII}:\quad \text{Stored}(d) \implies \text{Encrypted}(d, \text{AES256})$$

$$\forall d \in \text{Transit}:\quad \text{TLS}(d) \geq 1.2$$

### $\mathcal{S}_6$: Payment Card Storage

$$\nexists\, c \in \text{CardData}:\quad \text{Stored}(c)$$

*No card data stored. Tokenized via payment gateway. PCI-DSS compliance.*

### $\mathcal{S}_7$: FsAssay Security Rules

$$\forall f \in \mathcal{D} \cup \mathcal{S}:\quad \mathcal{F}_{\text{SEC}}(f) = \emptyset$$

*Zero security violations. FSA-SEC01 through FSA-SEC07 all pass.*

---

## $\S 9$. Testing Requirements

### $\mathcal{T}_1$: Property-Based Tests (FsCheck)

$$\forall sc \in \text{SmartConstructors}:\quad \text{FsCheck}(sc, n=1000) \models \mathcal{I}_{sc}$$

*Every smart constructor verified with 1000 random inputs.*

### $\mathcal{T}_2$: State Machine Tests

$$\forall \Sigma \in \{\Sigma_1, \Sigma_2, \Sigma_3, \Sigma_4\}:\quad \forall (q, e) \in Q \times E:\quad \delta(q, e) \text{ is tested}$$

*Every possible transition (valid AND invalid) is tested.*

### $\mathcal{T}_3$: Invariant Tests

$$\forall \mathcal{I} \in \{\mathcal{I}_1, ..., \mathcal{I}_{20}\}:\quad \text{FsCheck}(\mathcal{I}, n=1000) \models \top$$

*Every invariant verified with 1000 random scenarios.*

### $\mathcal{T}_4$: Negative Tests

$$\forall \text{rule} \in \mathcal{F}:\quad \exists\, \text{test}:\ \text{CleanCode} \rightarrow \text{Violations} = \emptyset$$

*Every FsAssay rule has a negative test proving clean code produces zero violations.*

### $\mathcal{T}_5$: FsAssay Adjudicate

$$\text{Precision} = \frac{|\text{TruePositives}|}{|\text{Predicted}|} \geq 0.95$$

$$\text{Recall} = \frac{|\text{TruePositives}|}{|\text{Expected}|} \geq 0.95$$

*FsAssay precision and recall ≥ 95% on the codebase.*

### $\mathcal{T}_6$: Integration Tests

$$\forall \text{flow} \in \{\text{OrderFlow}, \text{PartnerFlow}, \text{PaymentFlow}\}:\quad \text{EndToEnd}(\text{flow}) \models \top$$

*Full lifecycle tested: Create → Confirm → Assign → Pickup → Transit → Deliver → Settle.*

---

## $\S 10$. Database Schema (PostgreSQL)

```sql
-- Consumers
CREATE TABLE consumers (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    phone VARCHAR(15) NOT NULL UNIQUE,
    email VARCHAR(255) NOT NULL UNIQUE,
    name VARCHAR(100) NOT NULL,
    wallet_balance NUMERIC(10,2) NOT NULL DEFAULT 0,
    rating NUMERIC(2,1) NOT NULL DEFAULT 5.0,
    status VARCHAR(20) NOT NULL DEFAULT 'REGISTERED',
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),

    CONSTRAINT chk_phone CHECK (phone ~ '^[0-9]{10,15}$'),
    CONSTRAINT chk_email CHECK (email ~ '^[^@\s]+@[^@\s]+\.[^@\s]+$'),
    CONSTRAINT chk_wallet CHECK (wallet_balance >= 0),
    CONSTRAINT chk_rating CHECK (rating >= 1.0 AND rating <= 5.0),
    CONSTRAINT chk_status CHECK (status IN ('REGISTERED','VERIFIED','ACTIVE','SUSPENDED','DEACTIVATED'))
);

-- Partners
CREATE TABLE partners (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    phone VARCHAR(15) NOT NULL UNIQUE,
    email VARCHAR(255) NOT NULL UNIQUE,
    name VARCHAR(100) NOT NULL,
    pan VARCHAR(10) NOT NULL UNIQUE,
    vehicle_reg VARCHAR(20) NOT NULL,
    vehicle_type VARCHAR(20) NOT NULL,
    current_lat NUMERIC(9,6),
    current_lng NUMERIC(9,6),
    rating NUMERIC(2,1) NOT NULL DEFAULT 5.0,
    earnings NUMERIC(12,2) NOT NULL DEFAULT 0,
    kyc_status VARCHAR(20) NOT NULL DEFAULT 'PENDING',
    status VARCHAR(20) NOT NULL DEFAULT 'REGISTERED',
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),

    CONSTRAINT chk_partner_phone CHECK (phone ~ '^[0-9]{10,15}$'),
    CONSTRAINT chk_pan CHECK (pan ~ '^[A-Z]{5}[0-9]{4}[A-Z]$'),
    CONSTRAINT chk_vehicle_reg CHECK (vehicle_reg ~ '^[A-Z]{2}\s[0-9]{2}\s[A-Z]{1,2}\s[0-9]{4}$'),
    CONSTRAINT chk_vehicle_type CHECK (vehicle_type IN ('BICYCLE','BIKE','SCOOTER','CAR','AUTO','TRUCK')),
    CONSTRAINT chk_partner_rating CHECK (rating >= 1.0 AND rating <= 5.0),
    CONSTRAINT chk_earnings CHECK (earnings >= 0),
    CONSTRAINT chk_lat CHECK (current_lat IS NULL OR (current_lat >= -90 AND current_lat <= 90)),
    CONSTRAINT chk_lng CHECK (current_lng IS NULL OR (current_lng >= -180 AND current_lng <= 180)),
    CONSTRAINT chk_kyc CHECK (kyc_status IN ('PENDING','VERIFIED','REJECTED')),
    CONSTRAINT chk_partner_status CHECK (status IN ('REGISTERED','KYC_PENDING','KYC_VERIFIED','ONLINE','ON_TRIP','OFFLINE','SUSPENDED','DEACTIVATED'))
);

-- Orders
CREATE TABLE orders (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    consumer_id UUID NOT NULL REFERENCES consumers(id),
    partner_id UUID REFERENCES partners(id),
    service_type VARCHAR(20) NOT NULL,
    pickup_lat NUMERIC(9,6) NOT NULL,
    pickup_lng NUMERIC(9,6) NOT NULL,
    dropoff_lat NUMERIC(9,6) NOT NULL,
    dropoff_lng NUMERIC(9,6) NOT NULL,
    distance_km NUMERIC(6,2) NOT NULL,
    estimated_minutes INT NOT NULL,
    base_fare NUMERIC(8,2) NOT NULL,
    surge_multiplier NUMERIC(3,1) NOT NULL DEFAULT 1.0,
    total_fare NUMERIC(8,2) NOT NULL,
    payment_method VARCHAR(20) NOT NULL,
    payment_status VARCHAR(20) NOT NULL DEFAULT 'INITIATED',
    status VARCHAR(20) NOT NULL DEFAULT 'PLACED',
    otp VARCHAR(6) NOT NULL,
    cancellation_reason VARCHAR(50),
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    confirmed_at TIMESTAMPTZ,
    assigned_at TIMESTAMPTZ,
    picked_up_at TIMESTAMPTZ,
    in_transit_at TIMESTAMPTZ,
    delivered_at TIMESTAMPTZ,
    cancelled_at TIMESTAMPTZ,

    CONSTRAINT chk_service_type CHECK (service_type IN ('GROCERY','FOOD','PARCEL','RIDE','HOME_SERVICE','PHARMACY')),
    CONSTRAINT chk_distance CHECK (distance_km > 0 AND distance_km <= 50),
    CONSTRAINT chk_estimated CHECK (estimated_minutes >= 1 AND estimated_minutes <= 1440),
    CONSTRAINT chk_base_fare CHECK (base_fare >= 0),
    CONSTRAINT chk_surge CHECK (surge_multiplier >= 1.0 AND surge_multiplier <= 5.0),
    CONSTRAINT chk_total_fare CHECK (total_fare >= 0),
    CONSTRAINT chk_total_gte_base CHECK (total_fare >= base_fare),
    CONSTRAINT chk_payment_method CHECK (payment_method IN ('UPI','CARD','WALLET','CASH','NET_BANKING')),
    CONSTRAINT chk_payment_status CHECK (payment_status IN ('INITIATED','AUTHORIZED','CAPTURED','SETTLED','FAILED','REFUND_INITIATED','REFUNDED')),
    CONSTRAINT chk_order_status CHECK (status IN ('PLACED','CONFIRMED','PARTNER_ASSIGNED','PICKED_UP','IN_TRANSIT','DELIVERED','CANCELLED','REFUNDED')),
    CONSTRAINT chk_otp CHECK (otp ~ '^[0-9]{6}$'),
    CONSTRAINT chk_pickup_lat CHECK (pickup_lat >= -90 AND pickup_lat <= 90),
    CONSTRAINT chk_pickup_lng CHECK (pickup_lng >= -180 AND pickup_lng <= 180),
    CONSTRAINT chk_dropoff_lat CHECK (dropoff_lat >= -90 AND dropoff_lat <= 90),
    CONSTRAINT chk_dropoff_lng CHECK (dropoff_lng >= -180 AND dropoff_lng <= 180),
    CONSTRAINT chk_confirmed_before_assigned CHECK (assigned_at IS NULL OR confirmed_at IS NOT NULL),
    CONSTRAINT chk_assigned_before_picked CHECK (picked_up_at IS NULL OR assigned_at IS NOT NULL),
    CONSTRAINT chk_picked_before_transit CHECK (in_transit_at IS NULL OR picked_up_at IS NOT NULL),
    CONSTRAINT chk_transit_before_delivered CHECK (delivered_at IS NULL OR in_transit_at IS NOT NULL),
    CONSTRAINT chk_cancelled_no_delivery CHECK (status != 'CANCELLED' OR delivered_at IS NULL),
    CONSTRAINT chk_delivered_no_cancel CHECK (status != 'DELIVERED' OR cancelled_at IS NULL)
);

-- Payments
CREATE TABLE payments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id UUID NOT NULL REFERENCES orders(id),
    method VARCHAR(20) NOT NULL,
    amount NUMERIC(8,2) NOT NULL,
    status VARCHAR(20) NOT NULL DEFAULT 'INITIATED',
    gateway_ref VARCHAR(100),
    initiated_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    authorized_at TIMESTAMPTZ,
    captured_at TIMESTAMPTZ,
    settled_at TIMESTAMPTZ,
    refunded_at TIMESTAMPTZ,

    CONSTRAINT chk_payment_amount CHECK (amount > 0),
    CONSTRAINT chk_payment_status CHECK (status IN ('INITIATED','AUTHORIZED','CAPTURED','SETTLED','FAILED','REFUND_INITIATED','REFUNDED')),
    CONSTRAINT chk_auth_before_capture CHECK (captured_at IS NULL OR authorized_at IS NOT NULL),
    CONSTRAINT chk_capture_before_settle CHECK (settled_at IS NULL OR captured_at IS NOT NULL)
);

-- Ratings
CREATE TABLE ratings (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id UUID NOT NULL UNIQUE REFERENCES orders(id),
    rater_type VARCHAR(10) NOT NULL,
    rater_id UUID NOT NULL,
    rated_id UUID NOT NULL,
    score NUMERIC(2,1) NOT NULL,
    comment VARCHAR(500),
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),

    CONSTRAINT chk_rater_type CHECK (rater_type IN ('CONSUMER','PARTNER')),
    CONSTRAINT chk_score CHECK (score >= 1.0 AND score <= 5.0),
    CONSTRAINT chk_rating_window CHECK (created_at <= (SELECT delivered_at FROM orders WHERE id = order_id) + INTERVAL '24 hours')
);
```

---

## $\S 11$. Project Structure

```
urbanclan-core/
├── db/
│   └── init/
│       └── 01-schema.sql              ← §10 (PostgreSQL)
├── src/
│   └── Domain/
│       ├── Domain.fsproj
│       ├── Types.fs                   ← §2 (Smart Constructors)
│       ├── Entities.fs                ← §3 (Domain Entities)
│       ├── OrderStateMachine.fs       ← §4 Σ₁
│       ├── PartnerStateMachine.fs     ← §4 Σ₂
│       ├── PaymentStateMachine.fs     ← §4 Σ₃
│       ├── ConsumerStateMachine.fs    ← §4 Σ₄
│       ├── FareCalculation.fs         ← §5 I₁₂-I₁₅
│       ├── Validation.fs              ← §5 I₁₆-I₂₀
│       └── Library.fs                 ← Module exports
├── tests/
│   ├── Domain.Tests.fsproj
│   ├── SmartConstructorTests.fs       ← §9 T₁
│   ├── StateMachineTests.fs           ← §9 T₂
│   ├── InvariantTests.fs              ← §9 T₃
│   ├── NegativeTests.fs               ← §9 T₄
│   ├── IntegrationTests.fs            ← §9 T₆
│   └── Generators.fs                  ← CanonFlow FsCheck arbitraries
├── client/
│   └── src/
│       ├── validators.ts              ← CanonFlow Zod validators
│       └── types.ts                   ← CanonFlow TypeScript types
├── output/
│   ├── PROOF.md                       ← CanonFlow fidelity proof
│   ├── layer-map.md                   ← Architectural boundary map
│   ├── fsassay-scan.sarif             ← FsAssay scan output
│   ├── fsassay-scan.json              ← FsAssay JSON output
│   ├── ratecard.md                    ← FsAssay quality grade
│   └── toolchain-lock.json            ← Reproducible environment
├── scripts/
│   └── dogfood.sh                     ← Full pipeline automation
├── docker-compose.yml                 ← PostgreSQL 16
├── fs-assay.toml                      ← FsAssay configuration
├── .github/
│   └── workflows/
│       └── ci.yml                     ← CI: build + test + fsassay
└── README.md
```

---

## $\S 12$. FsAssay Configuration

```toml
# fs-assay.toml

[profile.production]
severity.FSA-SEC01 = "critical"    # Hard-coded secrets → BLOCK
severity.FSA-SEC02 = "critical"    # SQL injection → BLOCK
severity.FSA-SEC04 = "critical"    # Weak crypto → BLOCK
severity.FSA-SEC05 = "critical"    # Disabled SSL → BLOCK
severity.FSA1001 = "error"         # Mutable → BLOCK
severity.FSA1002 = "error"         # Option.get → BLOCK
severity.FSA1003 = "error"         # Null → BLOCK
severity.FSA1004 = "error"         # Primitive obsession → BLOCK
severity.FSA1006 = "error"         # Exception flow → BLOCK
severity.FSA2022 = "error"         # I/O in core → BLOCK
severity.FSA-AI01 = "warning"      # Dead code → WARN
severity.FSA-AI10 = "warning"      # Magic numbers → WARN
severity.FSA2012 = "warning"       # printfn → WARN

[profile.test]
disable = ["FSA2012", "FSA-AI10"]  # Tests can use printfn and literals

[adjudicate]
precision_threshold = 0.95
recall_threshold = 0.95
```

---

## $\S 13$. CI Pipeline

```yaml
# .github/workflows/ci.yml
name: UrbanClan CI
on: [push, pull_request]

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    services:
      postgres:
        image: postgres:16
        env:
          POSTGRES_PASSWORD: ${{ secrets.DB_PASSWORD }}
        ports: ["5432:5432"]
    steps:
      - uses: actions/checkout@v4
      - name: Build
        run: dotnet build
      - name: Test
        run: dotnet test
      - name: FsAssay Scan
        run: |
          dotnet tool install fsassay
          fsassay src/ -s output/fsassay-scan.sarif --adjudicate
      - name: Upload SARIF
        uses: github/codeql-action/upload-sarif@v3
        with:
          sarif_file: output/fsassay-scan.sarif
      - name: CanonFlow
        run: ./scripts/dogfood.sh
      - name: Verify PROOF.md
        run: |
          # Fail if any constraint is "Unsupported" without a manual guard
          grep -c "Unsupported" output/PROOF.md | grep -q "^0$" || exit 1
```

---

## $\S 14$. Summary of Counts

| Category | Count |
|---|---|
| Smart Constructors | 12 |
| Domain Entities | 4 (Consumer, Partner, Order, Payment) |
| State Machines | 4 ($\Sigma_1$–$\Sigma_4$) |
| Total States | 25 |
| Total Transitions | 31 |
| Business Invariants | 20 ($\mathcal{I}_1$–$\mathcal{I}_{20}$) |
| FsAssay Rules Applied | 24 |
| DB Constraints | 47 |
| Security Requirements | 7 ($\mathcal{S}_1$–$\mathcal{S}_7$) |
| Test Categories | 6 ($\mathcal{T}_1$–$\mathcal{T}_6$) |
| CanonFlow Artifacts | 6 ($\mathcal{C}_1$–$\mathcal{C}_6$) |

---

## $\S 15$. The Fundamental Theorem

$$\boxed{\forall x \in \text{UrbanClan}:\quad \text{Valid}(x) \iff \text{SmartConstructor}(x) = \text{Ok}(x) \;\wedge\; \mathcal{F}(x) = \emptyset \;\wedge\; \Sigma(x) \models \mathcal{I}}$$

*An entity is valid if and only if:*
1. *Its smart constructor accepts it (type-level validity)*
2. *FsAssay produces zero violations (architectural validity)*
3. *Its state machine satisfies all invariants (behavioral validity)*

$$\text{Invalid data} \notin \mathcal{D}$$

*Invalid data does not exist in the domain. It is rejected at the boundary. It never enters. It never propagates. It never reaches the database. It never reaches the consumer. It never reaches the partner.*

$$\blacksquare$$
