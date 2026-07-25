# $\mathcal{W}$EBUY — Formal Requirements Specification

## ONDC Quick-Commerce Platform · Consumer App × Seller App × Beckn Protocol

**Version:** $\nu_0$ · **Date:** 25 July 2026 · **Status:** Axiomatic · **Protocol:** ONDC Beckn v1.1

---

## $\S 0$. Notation

| Symbol | Meaning |
|---|---|
| $\mathcal{D}$ | Functional Core (pure domain) |
| $\mathcal{S}$ | Imperative Shell (I/O boundary) |
| $\mathcal{B}$ | Beckn Protocol Layer (ONDC messages) |
| $\mathcal{F}$ | FsAssay Rule Set |
| $\mathcal{C}$ | CanonFlow Pipeline |
| $\Sigma$ | State Machine |
| $\delta$ | Transition Function |
| $\mathcal{I}$ | Invariant Set |
| $\text{Ok}(x)$ / $\text{Err}(e)$ | Result constructors |
| $\square$ | Always (temporal) |
| $\diamond$ | Eventually (temporal) |
| $\forall, \exists, \nexists$ | Quantifiers |
| $\triangleq$ | Defined as |
| $\vdash$ | Entails |
| $\models$ | Satisfies |
| $\blacksquare$ | Q.E.D. |

---

## $\S 1$. ONDC Protocol Axioms (Beckn v1.1)

### Axiom 1.1 — Protocol Message Ordering

$$\forall \text{txn} \in \text{Transaction}:\quad \text{search} \prec \text{select} \prec \text{init} \prec \text{confirm} \prec \text{status}$$

*ONDC messages MUST follow this order. No message may precede its prerequisite.*

### Axiom 1.2 — Idempotency

$$\forall m \in \text{Message}:\quad \text{process}(m) \equiv \text{process}(m) \quad \text{(same message\_id)}$$

*Processing the same ONDC message twice produces the same result. No duplicate orders.*

### Axiom 1.3 — Async Acknowledgement

$$\forall m \in \text{Message}:\quad \text{ack}(m) \in \{\text{ACK}, \text{NACK}\} \;\wedge\; t_{\text{ack}} - t_{\text{recv}} \leq 30\text{s}$$

*Every ONDC message receives ACK/NACK within 30 seconds. The actual processing is asynchronous.*

### Axiom 1.4 — Digital Signature

$$\forall m \in \text{Message}:\quad \text{Valid}(m) \iff \text{Verify}(\text{Sign}(m),\ \text{PublicKey}_{\text{sender}}) = \top$$

*Every ONDC message is signed. Unsigned messages are rejected.*

### Axiom 1.5 — Interoperability

$$\forall \text{buyer} \in \text{ONDC Network}:\quad \text{Discoverable}(\text{Webuy Seller}) = \top$$

*Any ONDC buyer app can discover and transact with Webuy. No walled garden.*

### Axiom 1.6 — FCIS Purity

$$\forall f \in \mathcal{D}:\quad \text{IO}(f) = \emptyset \;\wedge\; \text{Mut}(f) = \emptyset \;\wedge\; \text{Exn}(f) = \emptyset$$

*The core is pure. ONDC message parsing, HTTP calls, DB writes — all in the shell.*

### Axiom 1.7 — FsAssay Enforcement

$$\forall f \in \mathcal{D}:\quad \mathcal{F}(f) = \emptyset$$

*Zero FsAssay violations in the core. Always.*

### Axiom 1.8 — CanonFlow Fidelity

$$\forall c \in \text{Constraints}(\text{DB}):\quad \text{CanonFlow}(c) \models c_{\text{TS}} \;\vee\; \text{Guard}(c) \in \mathcal{S}$$

---

## $\S 2$. Primitive Types (Smart Constructors)

### 2.1 — Core Identifiers

$$\text{PhoneNumber} \triangleq \{ p \in [0\text{-}9]^{10..15} \}$$

$$\text{Email} \triangleq \{ e \in \text{String} \mid \exists!\, @ \;\wedge\; \exists\, . \;\wedge\; \neg\text{ws} \}$$

$$\text{Pincode} \triangleq \{ p \in [0\text{-}9]^6 \mid p_1 \neq 0 \}$$

$$\text{GSTIN} \triangleq \{ g \in \text{String} \mid g \sim \texttt{[0-9]\{2\}[A-Z]\{5\}[0-9]\{4\}[A-Z][0-9][A-Z0-9][Z][0-9A-Z]} \}$$

$$\text{FSSAI} \triangleq \{ f \in \text{String} \mid f \sim \texttt{[0-9]\{14\}} \}$$

*Food Safety license — mandatory for grocery sellers.*

### 2.2 — Geo & Delivery

$$\text{GeoCoord} \triangleq \{ (\text{lat}, \text{lng}) \in \mathbb{R}^2 \mid -90 \leq \text{lat} \leq 90 \;\wedge\; -180 \leq \text{lng} \leq 180 \}$$

$$\text{Distance} \triangleq \{ d \in \mathbb{R} \mid 0 < d \leq 10 \}$$

*Max 10km — dark stores are every 2km.*

$$\text{DeliveryRadius} \triangleq \{ r \in \mathbb{R} \mid 0.5 \leq r \leq 5.0 \}$$

*Each dark store serves a 0.5–5km radius.*

$$\text{DeliveryETA} \triangleq \{ t \in \mathbb{N} \mid 5 \leq t \leq 120 \}$$

*5 minutes to 2 hours. Quick commerce.*

### 2.3 — Money & Pricing

$$\text{Money} \triangleq \{ m \in \mathbb{Q} \mid m \geq 0 \;\wedge\; \text{precision}(m) \leq 2 \;\wedge\; \neg\text{NaN} \;\wedge\; \neg\text{Inf} \}$$

$$\text{Quantity} \triangleq \{ q \in \mathbb{N} \mid 1 \leq q \leq 99 \}$$

$$\text{WeightGrams} \triangleq \{ w \in \mathbb{N} \mid 1 \leq w \leq 50000 \}$$

*1g to 50kg per item.*

$$\text{DiscountPct} \triangleq \{ d \in \mathbb{R} \mid 0 \leq d \leq 90 \}$$

*Max 90% discount.*

$$\text{SurgeMultiplier} \triangleq \{ s \in \mathbb{R} \mid 1.0 \leq s \leq 3.0 \}$$

*Quick-commerce surge maxes at 3× (not 5× like rides).*

### 2.4 — Product & Inventory

$$\text{SKU} \triangleq \{ s \in \text{String} \mid |s| \in [6, 30] \;\wedge\; s \sim \texttt{[A-Z0-9-]+} \}$$

$$\text{Barcode} \triangleq \{ b \in [0\text{-}9]^{8..14} \}$$

*EAN-8 or EAN-14.*

$$\text{ExpiryDate} \triangleq \{ d \in \text{Date} \mid d > \text{today} \}$$

*Cannot create a product with past expiry.*

$$\text{ShelfLifeDays} \triangleq \{ d \in \mathbb{N} \mid 1 \leq d \leq 3650 \}$$

$$\text{TemperatureCelsius} \triangleq \{ t \in \mathbb{R} \mid -25 \leq t \leq 60 \}$$

*Cold chain: -25°C (frozen) to 60°C (hot food).*

### 2.5 — Rating & Review

$$\text{Rating} \triangleq \{ r \in \mathbb{R} \mid 1.0 \leq r \leq 5.0 \;\wedge\; \text{precision}(r) \leq 1 \}$$

$$\text{OTP} \triangleq \{ o \in [0\text{-}9]^4 \}$$

*4-digit OTP for delivery handoff (quick-commerce standard).*

### 2.6 — ONDC Protocol Types

$$\text{TransactionId} \triangleq \{ t \in \text{UUID} \}$$

$$\text{MessageId} \triangleq \{ m \in \text{UUID} \}$$

$$\text{SubscriberId} \triangleq \{ s \in \text{String} \mid s \sim \texttt{[a-z0-9.]+} \}$$

*ONDC subscriber ID: e.g., `webuy.buyer` or `webuy.seller`.*

$$\text{BecknAPI} \triangleq \text{Search} \mid \text{Select} \mid \text{Init} \mid \text{Confirm} \mid \text{Status} \mid \text{Track} \mid \text{Cancel} \mid \text{Update} \mid \text{Rating} \mid \text{Support}$$

---

## $\S 3$. Domain Entities

### 3.1 — Product (30,000+ SKUs)

$$\text{Product} \triangleq \left\{ \begin{array}{l} \text{Id} : \text{Guid} \\ \text{SKU} : \text{SKU} \\ \text{Barcode} : \text{Barcode option} \\ \text{Name} : \text{NonEmptyString} \\ \text{Category} : \text{Category} \\ \text{SubCategory} : \text{SubCategory} \\ \text{Brand} : \text{NonEmptyString} \\ \text{MRP} : \text{Money} \\ \text{SellingPrice} : \text{Money} \\ \text{Discount} : \text{DiscountPct} \\ \text{Unit} : \text{Unit} \\ \text{Weight} : \text{WeightGrams option} \\ \text{Images} : \text{Uri}^+ \\ \text{Description} : \text{String} \\ \text{Expiry} : \text{ExpiryDate option} \\ \text{StorageTemp} : \text{TemperatureCelsius option} \\ \text{IsFSSAI} : \text{bool} \\ \text{FSSAILicense} : \text{FSSAI option} \\ \text{Tags} : \text{String}^* \\ \text{IsActive} : \text{bool} \\ \text{CreatedAt} : \text{Instant} \end{array} \right\}$$

### 3.2 — Category Hierarchy

$$\text{Category} \triangleq \left\{ \begin{array}{l} \text{Fruits} \mid \text{Vegetables} \mid \text{Dairy} \mid \text{Bakery} \mid \text{Snacks} \mid \text{Beverages} \\ \text{Atta\&Rice} \mid \text{Oils\&Masala} \mid \text{PersonalCare} \mid \text{Cleaning} \\ \text{BabyCare} \mid \text{PetFood} \mid \text{Electronics} \mid \text{Beauty} \\ \text{Pharmacy} \mid \text{Kitchen} \mid \text{Puja} \mid \text{Stationery} \\ \text{Printouts} \mid \text{HomeOffice} \mid \text{IceCream} \mid \text{Frozen} \end{array} \right\}$$

$$\text{Unit} \triangleq \text{Piece} \mid \text{Kg} \mid \text{Gram} \mid \text{Litre} \mid \text{Ml} \mid \text{Pack} \mid \text{Dozen} \mid \text{Bundle} \mid \text{Sheet}$$

### 3.3 — DarkStore (Every 2km)

$$\text{DarkStore} \triangleq \left\{ \begin{array}{l} \text{Id} : \text{Guid} \\ \text{Name} : \text{NonEmptyString} \\ \text{Location} : \text{GeoCoord} \\ \text{Address} : \text{Address} \\ \text{Pincode} : \text{Pincode} \\ \text{City} : \text{City} \\ \text{DeliveryRadius} : \text{DeliveryRadius} \\ \text{OperatingHours} : \text{TimeRange} \\ \text{Capacity} : \text{Capacity} \\ \text{ColdStorage} : \text{bool} \\ \text{FSSAILicense} : \text{FSSAI} \\ \text{GSTIN} : \text{GSTIN} \\ \text{Status} : \text{StoreStatus} \\ \text{Inventory} : \text{InventoryItem}^* \\ \text{AvgPickTime} : \text{Duration} \\ \text{Rating} : \text{Rating} \end{array} \right\}$$

### 3.4 — InventoryItem

$$\text{InventoryItem} \triangleq \left\{ \begin{array}{l} \text{ProductId} : \text{Guid} \\ \text{StoreId} : \text{Guid} \\ \text{Stock} : \text{Quantity} \\ \text{Reserved} : \text{Quantity} \\ \text{Available} : \text{Quantity} \\ \text{ShelfLocation} : \text{String} \\ \text{ExpiryBatch} : \text{ExpiryDate option} \\ \text{StorageTemp} : \text{TemperatureCelsius option} \\ \text{LastRestocked} : \text{Instant} \end{array} \right\}$$

**Invariant:**

$$\forall i \in \text{InventoryItem}:\quad i.\text{Available} = i.\text{Stock} - i.\text{Reserved} \;\wedge\; i.\text{Available} \geq 0$$

### 3.5 — Consumer

$$\text{Consumer} \triangleq \left\{ \begin{array}{l} \text{Id} : \text{Guid} \\ \text{Phone} : \text{PhoneNumber} \\ \text{Email} : \text{Email option} \\ \text{Name} : \text{NonEmptyString} \\ \text{Addresses} : \text{Address}^+ \\ \text{DefaultAddress} : \text{Address} \\ \text{Wallet} : \text{Money} \\ \text{Subscription} : \text{Subscription option} \\ \text{Preferences} : \text{Preferences} \\ \text{Status} : \text{ConsumerStatus} \\ \text{CreatedAt} : \text{Instant} \end{array} \right\}$$

### 3.6 — DeliveryPartner

$$\text{DeliveryPartner} \triangleq \left\{ \begin{array}{l} \text{Id} : \text{Guid} \\ \text{Phone} : \text{PhoneNumber} \\ \text{Name} : \text{NonEmptyString} \\ \text{VehicleType} : \text{VehicleType} \\ \text{VehicleReg} : \text{VehicleReg} \\ \text{CurrentLocation} : \text{GeoCoord} \\ \text{AssignedStore} : \text{Guid} \\ \text{Status} : \text{PartnerStatus} \\ \text{Rating} : \text{Rating} \\ \text{ActiveOrders} : \text{Guid}^* \\ \text{MaxConcurrent} : \text{Quantity} \\ \text{SpeedKmph} : \text{Speed} \\ \text{Earnings} : \text{Money} \\ \text{KYC} : \text{KYCStatus} \end{array} \right\}$$

**Invariant (Speed):**

$$\forall p \in \text{DeliveryPartner}:\quad p.\text{SpeedKmph} \leq 20$$

*Average driving speed is 20kmph. Safety first.*

### 3.7 — Order

$$\text{Order} \triangleq \left\{ \begin{array}{l} \text{Id} : \text{Guid} \\ \text{ONDC txn\_id} : \text{TransactionId} \\ \text{ONDC message\_id} : \text{MessageId} \\ \text{ConsumerId} : \text{Guid} \\ \text{StoreId} : \text{Guid} \\ \text{PartnerId} : \text{Guid option} \\ \text{Items} : \text{OrderItem}^+ \\ \text{SubTotal} : \text{Money} \\ \text{DeliveryFee} : \text{Money} \\ \text{SurgeFee} : \text{Money} \\ \text{Discount} : \text{Money} \\ \text{TotalFare} : \text{Money} \\ \text{PaymentMethod} : \text{PaymentMethod} \\ \text{PaymentStatus} : \text{PaymentStatus} \\ \text{Status} : \text{OrderStatus} \\ \text{OTP} : \text{OTP} \\ \text{DeliveryAddress} : \text{Address} \\ \text{DeliveryETA} : \text{DeliveryETA} \\ \text{Distance} : \text{Distance} \\ \text{CreatedAt} : \text{Instant} \\ \text{ConfirmedAt} : \text{Instant option} \\ \text{PackedAt} : \text{Instant option} \\ \text{PickedUpAt} : \text{Instant option} \\ \text{DeliveredAt} : \text{Instant option} \\ \text{CancelledAt} : \text{Instant option} \\ \text{CancellationReason} : \text{CancellationReason option} \end{array} \right\}$$

### 3.8 — OrderItem

$$\text{OrderItem} \triangleq \left\{ \begin{array}{l} \text{ProductId} : \text{Guid} \\ \text{SKU} : \text{SKU} \\ \text{Name} : \text{NonEmptyString} \\ \text{Quantity} : \text{Quantity} \\ \text{UnitPrice} : \text{Money} \\ \text{TotalPrice} : \text{Money} \\ \text{Weight} : \text{WeightGrams option} \\ \text{StorageTemp} : \text{TemperatureCelsius option} \\ \text{Substitution} : \text{SubstitutionPolicy} \end{array} \right\}$$

### 3.9 — Payment

$$\text{PaymentMethod} \triangleq \left\{ \begin{array}{l} \text{UPI} \mid \text{Card} \mid \text{Wallet} \mid \text{COD} \mid \text{NetBanking} \\ \mid \text{BNPL} \mid \text{Sodexo} \mid \text{PaytmFood} \mid \text{WebuyWallet} \end{array} \right\}$$

### 3.10 — Subscription (Daily Milk, Weekly Groceries)

$$\text{Subscription} \triangleq \left\{ \begin{array}{l} \text{Id} : \text{Guid} \\ \text{ConsumerId} : \text{Guid} \\ \text{Items} : \text{SubscriptionItem}^+ \\ \text{Frequency} : \text{Frequency} \\ \text{DeliveryTime} : \text{TimeRange} \\ \text{NextDelivery} : \text{Instant} \\ \text{Status} : \text{SubscriptionStatus} \\ \text{PaymentMethod} : \text{PaymentMethod} \\ \text{PauseUntil} : \text{Instant option} \end{array} \right\}$$

$$\text{Frequency} \triangleq \text{Daily} \mid \text{AlternateDays} \mid \text{Weekly} \mid \text{BiWeekly} \mid \text{Monthly}$$

### 3.11 — Printout Order (Unique Feature)

$$\text{PrintoutOrder} \triangleq \left\{ \begin{array}{l} \text{Id} : \text{Guid} \\ \text{ConsumerId} : \text{Guid} \\ \text{Documents} : \text{PrintDocument}^+ \\ \text{ColorMode} : \text{BW} \mid \text{Color} \\ \text{PaperSize} : \text{A4} \mid \text{A3} \mid \text{Letter} \\ \text{Copies} : \text{Quantity} \\ \text{SingleSided} : \text{bool} \\ \text{TotalPages} : \text{Quantity} \\ \text{TotalCost} : \text{Money} \\ \text{Status} : \text{OrderStatus} \end{array} \right\}$$

### 3.12 — City Coverage (20+ Cities)

$$\text{City} \triangleq \left\{ \begin{array}{l} \text{Ahmedabad} \mid \text{Bengaluru} \mid \text{Chandigarh} \mid \text{Chennai} \mid \text{Delhi} \\ \mid \text{Faridabad} \mid \text{Gurgaon} \mid \text{Hyderabad} \mid \text{Jaipur} \mid \text{Jalandhar} \\ \mid \text{Kanpur} \mid \text{Kolkata} \mid \text{Lucknow} \mid \text{Ludhiana} \mid \text{Meerut} \\ \mid \text{Mohali} \mid \text{Mumbai} \mid \text{Panchkula} \mid \text{Pune} \mid \text{Noida} \\ \mid \text{Ghaziabad} \mid \text{Vadodara} \mid \text{Zirakpur} \end{array} \right\}$$

---

## $\S 4$. State Machines

### $\Sigma_1$: Order Lifecycle (ONDC-Aligned)

$$Q_1 = \{\text{Created},\ \text{Accepted},\ \text{Packed},\ \text{PickedUp},\ \text{InTransit},\ \text{Delivered},\ \text{Cancelled},\ \text{Returned},\ \text{Refunded}\}$$

**ONDC Message → State Mapping:**

| ONDC Message | State Transition | Guard |
|---|---|---|
| `/confirm` → `/on_confirm` | Created → Accepted | $\text{Inventory available} \;\wedge\; \text{Payment authorized}$ |
| Internal | Accepted → Packed | $\text{All items picked} \;\wedge\; \text{Quality checked}$ |
| Internal | Packed → PickedUp | $\text{Partner assigned} \;\wedge\; \text{OTP verified}$ |
| `/track` → `/on_track` | PickedUp → InTransit | $\text{Partner location updated}$ |
| Internal | InTransit → Delivered | $\text{OTP verified} \;\wedge\; \text{GeoFence} < 100\text{m}$ |
| `/cancel` → `/on_cancel` | {Created, Accepted} → Cancelled | $\text{Cancellation window}$ |
| `/update` → `/on_update` | Delivered → Returned | $\text{Return window} < 24\text{h}$ |
| Internal | {Cancelled, Returned} → Refunded | $\text{Payment captured}$ |

**Invariant $\mathcal{I}_1$ (Temporal Ordering):**

$$\square(t_{\text{created}} \leq t_{\text{accepted}} \leq t_{\text{packed}} \leq t_{\text{picked}} \leq t_{\text{transit}} \leq t_{\text{delivered}})$$

**Invariant $\mathcal{I}_2$ (Quick Commerce SLA):**

$$\forall o \in \text{Order}:\quad o.\text{Status} = \text{Delivered} \implies t_{\text{delivered}} - t_{\text{created}} \leq o.\text{DeliveryETA}$$

*Delivery within promised ETA. This is the core promise.*

**Invariant $\mathcal{I}_3$ (Cancellation Window):**

$$\delta_1(\text{Created}, \text{Cancel}) = \text{Ok} \iff t_{\text{now}} - t_{\text{created}} < 60\text{s}$$

$$\delta_1(\text{Accepted}, \text{Cancel}) = \text{Ok} \iff t_{\text{now}} - t_{\text{created}} < 120\text{s} \;\wedge\; \text{Status} \neq \text{Packed}$$

*Free cancellation: 60s before acceptance, 120s after. Once packed, no cancellation.*

**Invariant $\mathcal{I}_4$ (OTP Gate):**

$$\delta_1(\text{Packed}, \text{Pickup}) = \text{Ok} \iff \text{OTP}_{\text{partner}} = \text{OTP}_{\text{order}}$$

**Invariant $\mathcal{I}_5$ (Geo-Fence at Delivery):**

$$\delta_1(\text{InTransit}, \text{Deliver}) = \text{Ok} \iff d(\text{Partner}_{\text{loc}},\ \text{Delivery}_{\text{coord}}) < 100\text{m}$$

*100m for quick-commerce (tighter than 200m for general delivery).*

**Invariant $\mathcal{I}_6$ (Return Window):**

$$\delta_1(\text{Delivered}, \text{Return}) = \text{Ok} \iff t_{\text{now}} - t_{\text{delivered}} < 86400\text{s}$$

*24-hour return window.*

---

### $\Sigma_2$: ONDC Transaction Lifecycle

$$Q_2 = \{\text{Searching},\ \text{Selected},\ \text{Initializing},\ \text{Confirmed},\ \text{Active},\ \text{Completed},\ \text{Cancelled}\}$$

| ONDC API | Transition |
|---|---|
| `/search` → `/on_search` | ∅ → Searching |
| `/select` → `/on_select` | Searching → Selected |
| `/init` → `/on_init` | Selected → Initializing |
| `/confirm` → `/on_confirm` | Initializing → Confirmed |
| `/status` → `/on_status` | Confirmed → Active |
| `/track` → `/on_track` | Active → Active (location update) |
| `/cancel` → `/on_cancel` | {Searching, Selected, Initializing, Confirmed} → Cancelled |
| `/update` → `/on_update` | Active → Active (modification) |
| `/rating` → `/on_rating` | Completed → Completed (feedback) |
| `/support` → `/on_support` | Any → Any (help) |

**Invariant $\mathcal{I}_7$ (ONDC Idempotency):**

$$\forall m_1, m_2 \in \text{Message}:\quad m_1.\text{message\_id} = m_2.\text{message\_id} \implies \text{process}(m_1) \equiv \text{process}(m_2)$$

**Invariant $\mathcal{I}_8$ (ONDC ACK Deadline):**

$$\forall m \in \text{Message}:\quad t_{\text{ack}} - t_{\text{recv}} \leq 30\text{s}$$

---

### $\Sigma_3$: Delivery Partner Lifecycle

$$Q_3 = \{\text{Registered},\ \text{KYCPending},\ \text{KYCVerified},\ \text{Online},\ \text{Assigned},\ \text{Picking},\ \text{Delivering},\ \text{Offline},\ \text{Suspended},\ \text{Deactivated}\}$$

**Invariant $\mathcal{I}_9$ (Max Concurrent Orders):**

$$\forall p \in \text{Partner}:\quad |p.\text{ActiveOrders}| \leq p.\text{MaxConcurrent}$$

*Quick-commerce: max 3 concurrent orders per partner.*

**Invariant $\mathcal{I}_{10}$ (Store Assignment):**

$$\forall p \in \text{Partner}:\quad p.\text{Status} \in \{\text{Online}, \text{Assigned}, \text{Picking}, \text{Delivering}\} \implies p.\text{AssignedStore} \neq \emptyset$$

*Every active partner is assigned to a dark store.*

**Invariant $\mathcal{I}_{11}$ (Speed Limit):**

$$\forall p \in \text{Partner}:\quad p.\text{SpeedKmph} \leq 20$$

---

### $\Sigma_4$: Payment Lifecycle

$$Q_4 = \{\text{Initiated},\ \text{Authorized},\ \text{Captured},\ \text{Settled},\ \text{Failed},\ \text{RefundInitiated},\ \text{Refunded}\}$$

**Invariant $\mathcal{I}_{12}$ (Capture Gate):**

$$\delta_4(\text{Authorized}, \text{Capture}) = \text{Ok} \iff \text{Order.Status} = \text{Delivered}$$

*Payment captured ONLY after delivery. Never before.*

**Invariant $\mathcal{I}_{13}$ (COD Gate):**

$$\text{PaymentMethod} = \text{COD} \implies \text{PaymentStatus} = \text{Captured} \iff \text{Order.Status} = \text{Delivered} \;\wedge\; \text{CashCollected} = \top$$

*COD: payment captured only when cash is physically collected at doorstep.*

**Invariant $\mathcal{I}_{14}$ (No Double Capture):**

$$\forall o \in \text{Order}:\quad |\{p \in \text{Payment} : p.\text{OrderId} = o.\text{Id} \;\wedge\; p.\text{Status} = \text{Captured}\}| \leq 1$$

---

### $\Sigma_5$: Inventory Lifecycle

$$Q_5 = \{\text{InStock},\ \text{LowStock},\ \text{Reserved},\ \text{OutOfStock},\ \text{Restocking},\ \text{Expired},\ \text{Discontinued}\}$$

**Invariant $\mathcal{I}_{15}$ (Stock Integrity):**

$$\forall i \in \text{InventoryItem}:\quad i.\text{Available} = i.\text{Stock} - i.\text{Reserved} \;\wedge\; i.\text{Available} \geq 0$$

**Invariant $\mathcal{I}_{16}$ (Expiry Guard):**

$$\forall i \in \text{InventoryItem}:\quad i.\text{ExpiryBatch} \neq \emptyset \;\wedge\; i.\text{ExpiryBatch} < \text{today} \implies i.\text{Status} = \text{Expired}$$

*Expired items are automatically removed from available stock.*

**Invariant $\mathcal{I}_{17}$ (Cold Chain):**

$$\forall i \in \text{InventoryItem}:\quad i.\text{StorageTemp} \neq \emptyset \implies \text{Store.ColdStorage} = \top$$

*Temperature-sensitive items can only be stored in stores with cold storage.*

---

### $\Sigma_6$: Subscription Lifecycle

$$Q_6 = \{\text{Active},\ \text{Paused},\ \text{Skipped},\ \text{Cancelled},\ \text{Expired}\}$$

**Invariant $\mathcal{I}_{18}$ (Auto-Order):**

$$\forall s \in \text{Subscription}:\quad s.\text{Status} = \text{Active} \;\wedge\; t_{\text{now}} \geq s.\text{NextDelivery} \implies \diamond(\text{Order created})$$

*Active subscriptions automatically create orders at the scheduled time.*

---

## $\S 5$. Business Invariants

### $\mathcal{I}_{19}$: Fare Calculation

$$\text{TotalFare} = \text{SubTotal} + \text{DeliveryFee} + \text{SurgeFee} - \text{Discount}$$

$$\text{DeliveryFee} = \begin{cases} 0 & \text{if } \text{SubTotal} \geq 199 \\ 25 & \text{if } 99 \leq \text{SubTotal} < 199 \\ 40 & \text{if } \text{SubTotal} < 99 \end{cases}$$

$$\text{SurgeFee} = \text{BaseDeliveryFee} \times (\text{SurgeMultiplier} - 1.0)$$

**Invariant:**

$$\forall o \in \text{Order}:\quad o.\text{TotalFare} \geq 0 \;\wedge\; o.\text{TotalFare} \geq o.\text{SubTotal} - o.\text{Discount}$$

### $\mathcal{I}_{20}$: Minimum Order Value

$$\forall o \in \text{Order}:\quad o.\text{SubTotal} \geq 99$$

*Minimum order value: ₹99.*

### $\mathcal{I}_{21}$: Store Distance

$$\forall o \in \text{Order}:\quad d(o.\text{Store}_{\text{loc}},\ o.\text{Delivery}_{\text{loc}}) \leq o.\text{Store}.\text{DeliveryRadius}$$

*Order must be within the store's delivery radius.*

### $\mathcal{I}_{22}$: Partner-Store Distance at Assignment

$$\forall o \in \text{Order}:\quad o.\text{Status} = \text{PickedUp} \implies d(\text{Partner}_{\text{loc}},\ \text{Store}_{\text{loc}}) \leq 1\text{km}$$

*Partner must be within 1km of the store at pickup.*

### $\mathcal{I}_{23}$: Weight Limit

$$\forall o \in \text{Order}:\quad \sum_{i \in o.\text{Items}} i.\text{Weight} \leq 15000\text{g}$$

*Max 15kg per order (bike delivery).*

### $\mathcal{I}_{24}$: Item Count Limit

$$\forall o \in \text{Order}:\quad |o.\text{Items}| \leq 50$$

*Max 50 items per order.*

### $\mathcal{I}_{25}$: FSSAI Compliance

$$\forall s \in \text{DarkStore}:\quad s.\text{Status} = \text{Active} \implies s.\text{FSSAILicense} \neq \emptyset \;\wedge\; \text{Valid}(s.\text{FSSAILicense})$$

*No store operates without a valid FSSAI license.*

### $\mathcal{I}_{26}$: Cold Chain Integrity

$$\forall o \in \text{Order}:\quad (\exists i \in o.\text{Items}:\ i.\text{StorageTemp} < 5°\text{C}) \implies \text{Partner.VehicleType} \in \{\text{Bike}, \text{Scooter}\} \;\wedge\; \text{InsulatedBag} = \top$$

*Cold items require insulated delivery.*

### $\mathcal{I}_{27}$: Subscription Delivery Window

$$\forall s \in \text{Subscription}:\quad s.\text{DeliveryTime} \in [06\text{:}00, 22\text{:}00]$$

*Subscriptions delivered between 6am and 10pm.*

### $\mathcal{I}_{28}$: Printout Size Limit

$$\forall p \in \text{PrintoutOrder}:\quad p.\text{TotalPages} \times p.\text{Copies} \leq 500$$

*Max 500 pages per printout order.*

### $\mathcal{I}_{29}$: ONDC Interoperability

$$\forall \text{buyer} \in \text{ONDC}:\quad \text{Search}(\text{buyer}, \text{Webuy}) \neq \emptyset \implies \text{Transactable}(\text{buyer}, \text{Webuy}) = \top$$

*If a buyer can discover Webuy, they can transact with Webuy. No discovery without transaction capability.*

### $\mathcal{I}_{30}$: Rating Window

$$\forall o \in \text{Order}:\quad \text{RatingAllowed}(o) \iff o.\text{Status} = \text{Delivered} \;\wedge\; t_{\text{now}} - t_{\text{delivered}} < 172800\text{s}$$

*48-hour rating window (longer than UrbanClan's 24h — grocery has longer tail).*

---

## $\S 6$. ONDC Message Flows

### 6.1 — Search Flow

```
Buyer App (Webuy Consumer)          ONDC Gateway          Seller App (Webuy Seller)
        │                                │                        │
        │── /search ──────────────────→  │                        │
        │   {intent: {item: {des: "milk"},│                        │
        │    fulfillment: {type: "Delivery"},                      │
        │    location: {lat, lng}}}      │                        │
        │                                │── /search ──────────→  │
        │                                │                        │
        │                                │  ←── /on_search ───────│
        │  ←── /on_search ──────────────│   {catalog: {items: [   │
        │   {catalog: {items: [...]}}    │     {id, name, price,   │
        │                                │      quantity, images}  │
        │                                │   ]}}                   │
```

### 6.2 — Order Flow

```
Buyer App                    Seller App                    Delivery
    │                            │                            │
    │── /select ──────────────→  │                            │
    │   {items: [{id, qty}]}     │                            │
    │  ←── /on_select ──────────│                            │
    │   {quote: {price, breakup}}│                            │
    │                            │                            │
    │── /init ────────────────→  │                            │
    │   {billing, fulfillment}   │                            │
    │  ←── /on_init ────────────│                            │
    │   {order: {id, quote}}     │                            │
    │                            │                            │
    │── /confirm ─────────────→  │                            │
    │   {order: {id, payment}}   │                            │
    │  ←── /on_confirm ─────────│                            │
    │   {order: {id, status}}    │                            │
    │                            │── assign partner ────────→  │
    │                            │                            │
    │── /status ──────────────→  │                            │
    │  ←── /on_status ──────────│                            │
    │   {order: {status: "Packed"}}                           │
    │                            │                            │
    │── /track ───────────────→  │                            │
    │  ←── /on_track ───────────│  ←── location update ──────│
    │   {tracking: {lat, lng, eta}}                           │
    │                            │                            │
    │  ←── /on_status ──────────│  ←── delivered ────────────│
    │   {order: {status: "Delivered"}}                        │
```

### 6.3 — Cancel Flow

```
Buyer App                    Seller App
    │                            │
    │── /cancel ──────────────→  │
    │   {order_id, reason}       │
    │  ←── /on_cancel ──────────│
    │   {order: {status: "Cancelled"}}
    │                            │
    │── /update ──────────────→  │  (if refund needed)
    │   {order_id, refund}       │
    │  ←── /on_update ──────────│
    │   {order: {refund: {status: "Initiated"}}}
```

### 6.4 — Rating & Support

```
Buyer App                    Seller App
    │                            │
    │── /rating ──────────────→  │
    │   {order_id, rating: 4.5}  │
    │  ←── /on_rating ──────────│
    │   {ack: "ACK"}             │
    │                            │
    │── /support ─────────────→  │
    │   {order_id, query}        │
    │  ←── /on_support ─────────│
    │   {support: {chat_url, phone}}
```

---

## $\S 7$. FsAssay Rule Mapping

| FsAssay Rule | Webuy Application | Enforcement |
|---|---|---|
| **FSA1001** (Mutable) | No `mutable` in fare calc, inventory update, state transitions | TAST |
| **FSA1002** (Option.get) | All optional fields (`PartnerId`, `Expiry`, `StorageTemp`) matched explicitly | TAST |
| **FSA1003** (Null) | No null. `Option` for absence. | TAST |
| **FSA1004** (Primitive Obsession) | 16 smart constructors replace all primitives | Regex |
| **FSA1006** (Exception Flow) | All ONDC handlers return `Result<'T, BecknError>` | TAST |
| **FSA1007** (Imperative Loops) | Inventory aggregation uses `List.fold`, not `while` | Regex |
| **FSA1008** (Inheritance) | No OOP. DUs + modules. | Regex |
| **FSA2008** (Async.RunSynchronously) | Only at `[<EntryPoint>]` | TAST |
| **FSA2012** (printfn) | No console in domain. Structured logging in shell. | TAST |
| **FSA2016** (Dependency Chain) | Max 5 layers: ONDC → API → Service → Domain → DB | Arch |
| **FSA2022** (Impure Core) | No HTTP, no DB, no Console in domain | TAST |
| **FSA-SEC01** (Secrets) | No API keys, UPI keys, DB passwords in code | Regex |
| **FSA-SEC02** (SQL Injection) | Parameterized queries only | Regex |
| **FSA-SEC04** (Weak Crypto) | SHA256+ for ONDC signatures. No MD5. | Regex |
| **FSA-SEC05** (SSL) | No disabled cert validation for ONDC gateway | Regex |
| **FSA-AI01** (Dead Code) | No unused functions | Regex |
| **FSA-AI05** (Inconsistent Errors) | One error strategy: `Result` everywhere | Regex |
| **FSA-AI10** (Magic Numbers) | All constants named: `minOrderValue`, `maxWeightKg`, `surgeMax` | Regex |

---

## $\S 8$. CanonFlow Pipeline

### $\mathcal{C}_1$: DB → F# Smart Constructors

$$\forall c \in \text{Constraints}(\text{DB}):\quad \exists\, sc \in \text{SmartConstructors}(\mathcal{D}):\quad sc \models c$$

### $\mathcal{C}_2$: F# → TypeScript (Zod + Types)

$$\forall sc \in \text{SmartConstructors}:\quad \text{CanonFlow}(sc) \rightarrow \text{ZodValidator}$$

### $\mathcal{C}_3$: F# → OpenAPI (ONDC Beckn Schema)

$$\forall t \in \text{Types}(\mathcal{D}):\quad \text{CanonFlow}(t) \rightarrow \text{OpenAPISchema}$$

### $\mathcal{C}_4$: DB → FsCheck Generators

$$\forall c \in \text{Constraints}(\text{DB}):\quad \text{CanonFlow}(c) \rightarrow \text{FsCheck.Arbitrary}$$

### $\mathcal{C}_5$: PROOF.md Generation

$$\text{CanonFlow}(\text{DB}) \rightarrow \text{PROOF.md} : \{c \mapsto (\text{Exact} \mid \text{Unsupported})\}$$

### $\mathcal{C}_6$: ONDC Beckn Schema Validation

$$\forall m \in \text{BecknMessage}:\quad \text{CanonFlow}(\text{Schema}) \models m$$

*Every ONDC message is validated against the Beckn v1.1 schema.*

---

## $\S 9$. Security Requirements

### $\mathcal{S}_1$: ONDC Digital Signature

$$\forall m \in \text{BecknMessage}:\quad \text{Valid}(m) \iff \text{Ed25519.Verify}(\text{Sign}(m),\ \text{PublicKey}_{\text{subscriber}}) = \top$$

### $\mathcal{S}_2$: Payment Security (PCI-DSS)

$$\nexists\, c \in \text{CardData}:\quad \text{Stored}(c)$$

*No card data stored. Tokenized via Razorpay/Cashfree.*

### $\mathcal{S}_3$: UPI Security

$$\forall p \in \text{UPIPayment}:\quad \text{VPA verified} \;\wedge\; \text{Amount locked} \;\wedge\; \text{MPIN required}$$

### $\mathcal{S}_4$: OTP Security

$$\forall o \in \text{OTP}:\quad \text{Attempts}(o) \leq 3 \implies \text{Valid}(o)$$

$$\text{Attempts}(o) > 3 \implies \text{Locked}(o) \;\wedge\; \text{LockDuration} = 300\text{s}$$

### $\mathcal{S}_5$: Rate Limiting

$$\forall u \in \text{User}:\quad |\{\text{req} : t_{\text{req}} \in [t_{\text{now}} - 60\text{s},\ t_{\text{now}}]\}| \leq 60$$

### $\mathcal{S}_6$: ONDC Subscriber Verification

$$\forall s \in \text{Subscriber}:\quad \text{Valid}(s) \iff \text{Registry.Lookup}(s.\text{subscriber\_id}) = \top \;\wedge\; \text{Valid}(s.\text{signing\_key})$$

### $\mathcal{S}_7$: Data Encryption

$$\forall d \in \text{PII}:\quad \text{Stored}(d) \implies \text{Encrypted}(d, \text{AES256})$$

$$\forall d \in \text{Transit}:\quad \text{TLS}(d) \geq 1.3$$

### $\mathcal{S}_8$: FSSAI Compliance

$$\forall s \in \text{DarkStore}:\quad \text{Active}(s) \implies \text{FSSAI.Valid}(s.\text{License}) \;\wedge\; \text{GST.Valid}(s.\text{GSTIN})$$

---

## $\S 10$. Testing Requirements

### $\mathcal{T}_1$: Smart Constructor Properties (FsCheck)

$$\forall sc \in \text{SmartConstructors}:\quad \text{FsCheck}(sc, n=1000) \models \mathcal{I}_{sc}$$

### $\mathcal{T}_2$: State Machine Transitions

$$\forall \Sigma \in \{\Sigma_1, ..., \Sigma_6\}:\quad \forall (q, e) \in Q \times E:\quad \delta(q, e) \text{ tested}$$

*6 machines × ~40 transitions each = ~240 transition tests.*

### $\mathcal{T}_3$: ONDC Protocol Tests

$$\forall \text{api} \in \text{BecknAPI}:\quad \text{ValidMessage}(\text{api}) \models \top \;\wedge\; \text{InvalidMessage}(\text{api}) \models \text{NACK}$$

*Every ONDC API tested with valid AND invalid messages.*

### $\mathcal{T}_4$: Invariant Tests

$$\forall \mathcal{I} \in \{\mathcal{I}_1, ..., \mathcal{I}_{30}\}:\quad \text{FsCheck}(\mathcal{I}, n=1000) \models \top$$

### $\mathcal{T}_5$: FsAssay Adjudicate

$$\text{Precision} \geq 0.95 \;\wedge\; \text{Recall} \geq 0.95$$

### $\mathcal{T}_6$: Integration Tests

$$\text{FullFlow}: \text{Search} \rightarrow \text{Select} \rightarrow \text{Init} \rightarrow \text{Confirm} \rightarrow \text{Pack} \rightarrow \text{Pickup} \rightarrow \text{Transit} \rightarrow \text{Deliver} \rightarrow \text{Settle}$$

### $\mathcal{T}_7$: Cold Chain Tests

$$\forall i \in \text{Items}:\quad i.\text{StorageTemp} < 5°\text{C} \implies \text{InsulatedBag} = \top \;\wedge\; \text{DeliveryTime} \leq 30\text{min}$$

### $\mathcal{T}_8$: ONDC Interoperability Tests

$$\forall \text{buyer} \in \text{MockBuyerApps}:\quad \text{FullFlow}(\text{buyer}, \text{Webuy}) \models \top$$

*Test with multiple mock ONDC buyer apps to verify interoperability.*

---

## $\S 11$. Database Schema (PostgreSQL)

```sql
-- Products (30,000+ SKUs)
CREATE TABLE products (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    sku VARCHAR(30) NOT NULL UNIQUE,
    barcode VARCHAR(14),
    name VARCHAR(200) NOT NULL,
    category VARCHAR(30) NOT NULL,
    sub_category VARCHAR(50),
    brand VARCHAR(100) NOT NULL,
    mrp NUMERIC(8,2) NOT NULL,
    selling_price NUMERIC(8,2) NOT NULL,
    discount_pct NUMERIC(4,1) NOT NULL DEFAULT 0,
    unit VARCHAR(20) NOT NULL,
    weight_grams INT,
    images TEXT[] NOT NULL DEFAULT '{}',
    description TEXT,
    expiry_date DATE,
    storage_temp NUMERIC(4,1),
    fssai_required BOOLEAN NOT NULL DEFAULT false,
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),

    CONSTRAINT chk_mrp CHECK (mrp > 0),
    CONSTRAINT chk_selling CHECK (selling_price > 0 AND selling_price <= mrp),
    CONSTRAINT chk_discount CHECK (discount_pct >= 0 AND discount_pct <= 90),
    CONSTRAINT chk_weight CHECK (weight_grams IS NULL OR (weight_grams >= 1 AND weight_grams <= 50000)),
    CONSTRAINT chk_storage_temp CHECK (storage_temp IS NULL OR (storage_temp >= -25 AND storage_temp <= 60)),
    CONSTRAINT chk_expiry CHECK (expiry_date IS NULL OR expiry_date > CURRENT_DATE),
    CONSTRAINT chk_category CHECK (category IN ('FRUITS','VEGETABLES','DAIRY','BAKERY','SNACKS',
        'BEVERAGES','ATTA_RICE','OILS_MASALA','PERSONAL_CARE','CLEANING','BABY_CARE','PET_FOOD',
        'ELECTRONICS','BEAUTY','PHARMACY','KITCHEN','PUJA','STATIONERY','PRINTOUTS',
        'HOME_OFFICE','ICE_CREAM','FROZEN'))
);

-- Dark Stores (every 2km)
CREATE TABLE dark_stores (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) NOT NULL,
    lat NUMERIC(9,6) NOT NULL,
    lng NUMERIC(9,6) NOT NULL,
    address TEXT NOT NULL,
    pincode VARCHAR(6) NOT NULL,
    city VARCHAR(30) NOT NULL,
    delivery_radius_km NUMERIC(3,1) NOT NULL DEFAULT 2.0,
    open_time TIME NOT NULL DEFAULT '06:00',
    close_time TIME NOT NULL DEFAULT '23:00',
    cold_storage BOOLEAN NOT NULL DEFAULT false,
    fssai_license VARCHAR(14) NOT NULL,
    gstin VARCHAR(15) NOT NULL,
    status VARCHAR(20) NOT NULL DEFAULT 'ACTIVE',
    avg_pick_minutes INT NOT NULL DEFAULT 5,
    rating NUMERIC(2,1) NOT NULL DEFAULT 5.0,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),

    CONSTRAINT chk_lat CHECK (lat >= -90 AND lat <= 90),
    CONSTRAINT chk_lng CHECK (lng >= -180 AND lng <= 180),
    CONSTRAINT chk_radius CHECK (delivery_radius_km >= 0.5 AND delivery_radius_km <= 5.0),
    CONSTRAINT chk_pincode CHECK (pincode ~ '^[1-9][0-9]{5}$'),
    CONSTRAINT chk_fssai CHECK (fssai_license ~ '^[0-9]{14}$'),
    CONSTRAINT chk_gstin CHECK (gstin ~ '^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z][0-9][A-Z0-9]Z[0-9A-Z]$'),
    CONSTRAINT chk_rating CHECK (rating >= 1.0 AND rating <= 5.0),
    CONSTRAINT chk_city CHECK (city IN ('AHMEDABAD','BENGALURU','CHANDIGARH','CHENNAI','DELHI',
        'FARIDABAD','GURGAON','HYDERABAD','JAIPUR','JALANDHAR','KANPUR','KOLKATA','LUCKNOW',
        'LUDHIANA','MEERUT','MOHALI','MUMBAI','PANCHKULA','PUNE','NOIDA','GHAZIABAD',
        'VADODARA','ZIRAKPUR'))
);

-- Inventory
CREATE TABLE inventory (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    product_id UUID NOT NULL REFERENCES products(id),
    store_id UUID NOT NULL REFERENCES dark_stores(id),
    stock INT NOT NULL DEFAULT 0,
    reserved INT NOT NULL DEFAULT 0,
    shelf_location VARCHAR(20),
    expiry_batch DATE,
    last_restocked TIMESTAMPTZ NOT NULL DEFAULT now(),

    CONSTRAINT chk_stock CHECK (stock >= 0),
    CONSTRAINT chk_reserved CHECK (reserved >= 0 AND reserved <= stock),
    CONSTRAINT chk_available CHECK (stock - reserved >= 0),
    CONSTRAINT chk_cold_chain CHECK (
        (SELECT storage_temp FROM products WHERE id = product_id) IS NULL
        OR (SELECT storage_temp FROM products WHERE id = product_id) >= 5
        OR (SELECT cold_storage FROM dark_stores WHERE id = store_id) = true
    ),
    UNIQUE(product_id, store_id)
);

-- Consumers
CREATE TABLE consumers (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    phone VARCHAR(15) NOT NULL UNIQUE,
    email VARCHAR(255),
    name VARCHAR(100) NOT NULL,
    wallet_balance NUMERIC(10,2) NOT NULL DEFAULT 0,
    status VARCHAR(20) NOT NULL DEFAULT 'REGISTERED',
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),

    CONSTRAINT chk_phone CHECK (phone ~ '^[0-9]{10,15}$'),
    CONSTRAINT chk_email CHECK (email IS NULL OR email ~ '^[^@\s]+@[^@\s]+\.[^@\s]+$'),
    CONSTRAINT chk_wallet CHECK (wallet_balance >= 0),
    CONSTRAINT chk_status CHECK (status IN ('REGISTERED','VERIFIED','ACTIVE','SUSPENDED','DEACTIVATED'))
);

-- Delivery Partners
CREATE TABLE delivery_partners (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    phone VARCHAR(15) NOT NULL UNIQUE,
    name VARCHAR(100) NOT NULL,
    vehicle_type VARCHAR(20) NOT NULL,
    vehicle_reg VARCHAR(20) NOT NULL,
    assigned_store UUID REFERENCES dark_stores(id),
    current_lat NUMERIC(9,6),
    current_lng NUMERIC(9,6),
    rating NUMERIC(2,1) NOT NULL DEFAULT 5.0,
    earnings NUMERIC(12,2) NOT NULL DEFAULT 0,
    max_concurrent INT NOT NULL DEFAULT 3,
    kyc_status VARCHAR(20) NOT NULL DEFAULT 'PENDING',
    status VARCHAR(20) NOT NULL DEFAULT 'REGISTERED',
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),

    CONSTRAINT chk_vehicle_type CHECK (vehicle_type IN ('BICYCLE','BIKE','SCOOTER','CAR','AUTO')),
    CONSTRAINT chk_vehicle_reg CHECK (vehicle_reg ~ '^[A-Z]{2}\s[0-9]{2}\s[A-Z]{1,2}\s[0-9]{4}$'),
    CONSTRAINT chk_rating CHECK (rating >= 1.0 AND rating <= 5.0),
    CONSTRAINT chk_earnings CHECK (earnings >= 0),
    CONSTRAINT chk_max_concurrent CHECK (max_concurrent >= 1 AND max_concurrent <= 5),
    CONSTRAINT chk_kyc CHECK (kyc_status IN ('PENDING','VERIFIED','REJECTED')),
    CONSTRAINT chk_partner_status CHECK (status IN ('REGISTERED','KYC_PENDING','KYC_VERIFIED',
        'ONLINE','ASSIGNED','PICKING','DELIVERING','OFFLINE','SUSPENDED','DEACTIVATED'))
);

-- Orders
CREATE TABLE orders (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    ondc_txn_id UUID NOT NULL,
    ondc_message_id UUID NOT NULL,
    consumer_id UUID NOT NULL REFERENCES consumers(id),
    store_id UUID NOT NULL REFERENCES dark_stores(id),
    partner_id UUID REFERENCES delivery_partners(id),
    sub_total NUMERIC(8,2) NOT NULL,
    delivery_fee NUMERIC(6,2) NOT NULL DEFAULT 0,
    surge_fee NUMERIC(6,2) NOT NULL DEFAULT 0,
    discount NUMERIC(8,2) NOT NULL DEFAULT 0,
    total_fare NUMERIC(8,2) NOT NULL,
    payment_method VARCHAR(20) NOT NULL,
    payment_status VARCHAR(20) NOT NULL DEFAULT 'INITIATED',
    status VARCHAR(20) NOT NULL DEFAULT 'CREATED',
    otp VARCHAR(4) NOT NULL,
    delivery_address TEXT NOT NULL,
    delivery_lat NUMERIC(9,6) NOT NULL,
    delivery_lng NUMERIC(9,6) NOT NULL,
    delivery_eta_minutes INT NOT NULL,
    distance_km NUMERIC(5,2) NOT NULL,
    total_weight_grams INT,
    item_count INT NOT NULL,
    cancellation_reason VARCHAR(100),
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    confirmed_at TIMESTAMPTZ,
    packed_at TIMESTAMPTZ,
    picked_up_at TIMESTAMPTZ,
    in_transit_at TIMESTAMPTZ,
    delivered_at TIMESTAMPTZ,
    cancelled_at TIMESTAMPTZ,

    CONSTRAINT chk_sub_total CHECK (sub_total >= 99),
    CONSTRAINT chk_delivery_fee CHECK (delivery_fee >= 0),
    CONSTRAINT chk_surge_fee CHECK (surge_fee >= 0),
    CONSTRAINT chk_discount CHECK (discount >= 0),
    CONSTRAINT chk_total CHECK (total_fare >= 0 AND total_fare >= sub_total - discount),
    CONSTRAINT chk_otp CHECK (otp ~ '^[0-9]{4}$'),
    CONSTRAINT chk_eta CHECK (delivery_eta_minutes >= 5 AND delivery_eta_minutes <= 120),
    CONSTRAINT chk_distance CHECK (distance_km > 0 AND distance_km <= 10),
    CONSTRAINT chk_weight CHECK (total_weight_grams IS NULL OR total_weight_grams <= 15000),
    CONSTRAINT chk_item_count CHECK (item_count >= 1 AND item_count <= 50),
    CONSTRAINT chk_payment_method CHECK (payment_method IN ('UPI','CARD','WALLET','COD',
        'NET_BANKING','BNPL','SODEXO','PAYTM_FOOD','WEBUY_WALLET')),
    CONSTRAINT chk_payment_status CHECK (payment_status IN ('INITIATED','AUTHORIZED','CAPTURED',
        'SETTLED','FAILED','REFUND_INITIATED','REFUNDED')),
    CONSTRAINT chk_order_status CHECK (status IN ('CREATED','ACCEPTED','PACKED','PICKED_UP',
        'IN_TRANSIT','DELIVERED','CANCELLED','RETURNED','REFUNDED')),
    CONSTRAINT chk_confirmed_before_packed CHECK (packed_at IS NULL OR confirmed_at IS NOT NULL),
    CONSTRAINT chk_packed_before_picked CHECK (picked_up_at IS NULL OR packed_at IS NOT NULL),
    CONSTRAINT chk_picked_before_transit CHECK (in_transit_at IS NULL OR picked_up_at IS NOT NULL),
    CONSTRAINT chk_transit_before_delivered CHECK (delivered_at IS NULL OR in_transit_at IS NOT NULL),
    CONSTRAINT chk_cancelled_no_delivery CHECK (status != 'CANCELLED' OR delivered_at IS NULL),
    CONSTRAINT chk_delivered_no_cancel CHECK (status != 'DELIVERED' OR cancelled_at IS NULL),
    CONSTRAINT chk_store_distance CHECK (distance_km <= (
        SELECT delivery_radius_km FROM dark_stores WHERE id = store_id
    ))
);

-- Order Items
CREATE TABLE order_items (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id UUID NOT NULL REFERENCES orders(id),
    product_id UUID NOT NULL REFERENCES products(id),
    sku VARCHAR(30) NOT NULL,
    name VARCHAR(200) NOT NULL,
    quantity INT NOT NULL,
    unit_price NUMERIC(8,2) NOT NULL,
    total_price NUMERIC(8,2) NOT NULL,
    weight_grams INT,
    storage_temp NUMERIC(4,1),
    substitution_policy VARCHAR(20) NOT NULL DEFAULT 'NO_SUBSTITUTE',

    CONSTRAINT chk_quantity CHECK (quantity >= 1 AND quantity <= 99),
    CONSTRAINT chk_unit_price CHECK (unit_price > 0),
    CONSTRAINT chk_total_price CHECK (total_price = unit_price * quantity),
    CONSTRAINT chk_substitution CHECK (substitution_policy IN ('NO_SUBSTITUTE','ALLOW_SIMILAR','ALLOW_ANY','CALL_ME'))
);

-- Payments
CREATE TABLE payments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id UUID NOT NULL REFERENCES orders(id),
    method VARCHAR(20) NOT NULL,
    amount NUMERIC(8,2) NOT NULL,
    status VARCHAR(20) NOT NULL DEFAULT 'INITIATED',
    gateway_ref VARCHAR(100),
    upi_txn_id VARCHAR(50),
    initiated_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    authorized_at TIMESTAMPTZ,
    captured_at TIMESTAMPTZ,
    settled_at TIMESTAMPTZ,
    refunded_at TIMESTAMPTZ,

    CONSTRAINT chk_amount CHECK (amount > 0),
    CONSTRAINT chk_payment_status CHECK (status IN ('INITIATED','AUTHORIZED','CAPTURED',
        'SETTLED','FAILED','REFUND_INITIATED','REFUNDED')),
    CONSTRAINT chk_auth_before_capture CHECK (captured_at IS NULL OR authorized_at IS NOT NULL),
    CONSTRAINT chk_capture_before_settle CHECK (settled_at IS NULL OR captured_at IS NOT NULL)
);

-- Subscriptions
CREATE TABLE subscriptions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    consumer_id UUID NOT NULL REFERENCES consumers(id),
    frequency VARCHAR(20) NOT NULL,
    delivery_start TIME NOT NULL DEFAULT '06:00',
    delivery_end TIME NOT NULL DEFAULT '08:00',
    next_delivery TIMESTAMPTZ NOT NULL,
    status VARCHAR(20) NOT NULL DEFAULT 'ACTIVE',
    payment_method VARCHAR(20) NOT NULL,
    pause_until TIMESTAMPTZ,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),

    CONSTRAINT chk_frequency CHECK (frequency IN ('DAILY','ALTERNATE_DAYS','WEEKLY','BIWEEKLY','MONTHLY')),
    CONSTRAINT chk_delivery_window CHECK (delivery_start >= '06:00' AND delivery_end <= '22:00'),
    CONSTRAINT chk_sub_status CHECK (status IN ('ACTIVE','PAUSED','SKIPPED','CANCELLED','EXPIRED'))
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
    CONSTRAINT chk_rating_window CHECK (created_at <= (
        SELECT delivered_at FROM orders WHERE id = order_id
    ) + INTERVAL '48 hours')
);

-- ONDC Transaction Log
CREATE TABLE ondc_transactions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    txn_id UUID NOT NULL,
    message_id UUID NOT NULL,
    api VARCHAR(20) NOT NULL,
    direction VARCHAR(10) NOT NULL,
    buyer_subscriber_id VARCHAR(50) NOT NULL,
    seller_subscriber_id VARCHAR(50) NOT NULL,
    payload JSONB NOT NULL,
    signature TEXT NOT NULL,
    ack_status VARCHAR(10) NOT NULL DEFAULT 'PENDING',
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),

    CONSTRAINT chk_api CHECK (api IN ('SEARCH','SELECT','INIT','CONFIRM','STATUS',
        'TRACK','CANCEL','UPDATE','RATING','SUPPORT')),
    CONSTRAINT chk_direction CHECK (direction IN ('REQUEST','RESPONSE')),
    CONSTRAINT chk_ack CHECK (ack_status IN ('PENDING','ACK','NACK')),
    CONSTRAINT chk_ack_deadline CHECK (
        ack_status != 'PENDING' OR created_at > now() - INTERVAL '30 seconds'
    )
);
```

---

## $\S 12$. Project Structure

```
webuy-core/
├── db/
│   └── init/
│       └── 01-schema.sql                    ← §11
├── src/
│   └── Domain/
│       ├── Domain.fsproj
│       ├── Types.fs                         ← §2 (16 Smart Constructors)
│       ├── Entities.fs                      ← §3 (12 Entities)
│       ├── OrderStateMachine.fs             ← §4 Σ₁
│       ├── ONDCLifecycle.fs                 ← §4 Σ₂
│       ├── PartnerStateMachine.fs           ← §4 Σ₃
│       ├── PaymentStateMachine.fs           ← §4 Σ₄
│       ├── InventoryStateMachine.fs         ← §4 Σ₅
│       ├── SubscriptionStateMachine.fs      ← §4 Σ₆
│       ├── FareCalculation.fs               ← §5 I₁₉
│       ├── InventoryRules.fs                ← §5 I₁₅-I₁₇
│       ├── DeliveryRules.fs                 ← §5 I₂₁-I₂₆
│       ├── ONDCMessages.fs                  ← §6 (Beckn message types)
│       ├── ONDCValidation.fs                ← §6 (message validation)
│       └── Library.fs
├── tests/
│   ├── Domain.Tests.fsproj
│   ├── SmartConstructorTests.fs             ← §10 T₁
│   ├── StateMachineTests.fs                 ← §10 T₂
│   ├── ONDCProtocolTests.fs                 ← §10 T₃
│   ├── InvariantTests.fs                    ← §10 T₄
│   ├── IntegrationTests.fs                  ← §10 T₆
│   ├── ColdChainTests.fs                    ← §10 T₇
│   ├── InteroperabilityTests.fs             ← §10 T₈
│   └── Generators.fs                        ← CanonFlow FsCheck arbitraries
├── client/
│   └── src/
│       ├── validators.ts                    ← CanonFlow Zod
│       ├── types.ts                         ← CanonFlow TS types
│       └── ondc-client.ts                   ← ONDC Beckn client
├── output/
│   ├── PROOF.md                             ← CanonFlow fidelity
│   ├── layer-map.md                         ← Architecture
│   ├── fsassay-scan.sarif                   ← FsAssay output
│   ├── ratecard.md                          ← Quality grade
│   └── toolchain-lock.json                  ← Reproducible env
├── scripts/
│   └── dogfood.sh
├── docker-compose.yml                       ← PostgreSQL 16
├── fs-assay.toml                            ← §7 config
├── .github/workflows/ci.yml                 ← §13
└── README.md
```

---

## $\S 13$. CI Pipeline

```yaml
name: Webuy CI
on: [push, pull_request]
jobs:
  build-test-verify:
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
      - name: Test (FsCheck + State Machines + ONDC)
        run: dotnet test --verbosity normal
      - name: FsAssay Scan
        run: |
          dotnet tool install fsassay
          fsassay src/ -s output/fsassay-scan.sarif --adjudicate
      - name: CanonFlow
        run: ./scripts/dogfood.sh
      - name: Verify PROOF.md (no unsupported without guard)
        run: |
          UNSUPPORTED=$(grep -c "Unsupported" output/PROOF.md || true)
          GUARDED=$(grep -c "Manual Guard" output/PROOF.md || true)
          if [ "$UNSUPPORTED" -gt "$GUARDED" ]; then
            echo "ERROR: Unsupported constraints without manual guards"
            exit 1
          fi
      - name: ONDC Schema Validation
        run: dotnet test --filter "ONDCProtocol"
      - name: Upload SARIF
        uses: github/codeql-action/upload-sarif@v3
        with:
          sarif_file: output/fsassay-scan.sarif
```

---

## $\S 14$. Summary

| Category | Count |
|---|---|
| Smart Constructors | 16 |
| Domain Entities | 12 |
| State Machines | 6 ($\Sigma_1$–$\Sigma_6$) |
| Total States | 42 |
| Total Transitions | ~55 |
| Business Invariants | 30 ($\mathcal{I}_1$–$\mathcal{I}_{30}$) |
| ONDC APIs | 10 (Beckn v1.1) |
| FsAssay Rules Applied | 24 |
| DB Constraints | 67 |
| Security Requirements | 8 ($\mathcal{S}_1$–$\mathcal{S}_8$) |
| Test Categories | 8 ($\mathcal{T}_1$–$\mathcal{T}_8$) |
| CanonFlow Artifacts | 6 ($\mathcal{C}_1$–$\mathcal{C}_6$) |
| Cities | 23 |
| Product Categories | 22 |
| Payment Methods | 9 |

---

## $\S 15$. The Fundamental Theorem

$$\boxed{\forall x \in \text{Webuy}:\quad \text{Valid}(x) \iff \underbrace{\text{SC}(x) = \text{Ok}}_{\text{CanonFlow}} \;\wedge\; \underbrace{\mathcal{F}(x) = \emptyset}_{\text{FsAssay}} \;\wedge\; \underbrace{\Sigma(x) \models \mathcal{I}}_{\text{State Machine}} \;\wedge\; \underbrace{\text{Beckn}(x) \models \top}_{\text{ONDC}}}$$

*An entity is valid if and only if:*
1. *Its smart constructor accepts it (CanonFlow)*
2. *FsAssay produces zero violations (Architecture)*
3. *Its state machine satisfies all invariants (Behavior)*
4. *Its ONDC messages conform to Beckn v1.1 (Protocol)*

$$\text{Invalid data} \notin \mathcal{D}$$

$$\text{Invalid architecture} \notin \text{CI}$$

$$\text{Invalid state} \notin \text{DB}$$

$$\text{Invalid message} \notin \text{ONDC}$$

*Four gates. Four enforcers. Zero invalid data reaches the consumer's doorstep.*

$$\blacksquare$$
