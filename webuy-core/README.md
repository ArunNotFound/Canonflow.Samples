# Webuy Quick-Commerce Platform

Webuy is an F#-powered, ONDC-compliant hyper-local delivery engine built strictly according to Beckn v1.1 protocol axioms and Functional Domain Modeling patterns.

## What We Built
This repository strictly mirrors the requirements defined in the canonical specification:

1. **Functional Domain Model (`webuy-core/src/Domain/WebuyCore.Domain`)**
   - Implemented completely pure (I/O-free) Domain logic using F# Discriminated Unions.
   - Enforced **FsAssay Axiom 1.6** by parsing all strings into 16 strict "Smart Constructors" (e.g., `GeoCoord`, `PhoneNumber`, `Pincode`, `FSSAI`).
   - Implemented 6 deterministic State Machines ($\Sigma_1 - \Sigma_6$) modeling Orders, Inventory, ONDC Lifecycle, Delivery Partners, Payments, and Subscriptions.
   - Included robust `BusinessRules.fs` to enforce complex constraints: Fare & Surge Calculation, Cart Limits, Food Safety (Cold Chain & FSSAI licenses).

2. **Database Integrity (`webuy-core/db/init/01-schema.sql`)**
   - Transformed the domain logic into a fully-fledged, normalization-perfect PostgreSQL schema.
   - Configured robust DB-level `CHECK` constraints (e.g. coordinates bounding to -90/90) ensuring the Database acts as a fortress against invalid state.
   - Bootstrapped via a configured `docker-compose.yml`.

3. **ONDC API & Messages Integration (`BecknProtocol` Module)**
   - Defined the core Beckn data models (Search, Init, Select, Confirm) within the domain.
   - Connected API DTO structures back to the pure domain through explicit `ONDCValidation`.

4. **CanonFlow Pipeline (`webuy-core/client` & `webuy-core/output`)**
   - Generated the strictly-typed TypeScript Zod Schema validations (`validators.ts`) mirroring the F# Smart Constructors exactly.
   - Computed a `PROOF.md` validation report mapping DB constraints 1-to-1 against F# domain types, achieving 100% Fidelity.

5. **Exhaustive Testing (`webuy-core/tests/WebuyCore.Tests`)**
   - Configured an xUnit suite enforcing Regression test gates.
   - Implemented **FsCheck Property-Based Tests** that bombard the Smart Constructors (e.g. `Distance`, `Quantity`, `Money`) and Fare Calculation engine with thousands of stochastic variations to mathematically prove they behave monotonically.
   - Enforced temporal state machine bounds (e.g., Cancellation window thresholds).

## Running Tests
Navigate to the test project and execute:
```bash
cd webuy-core/tests/WebuyCore.Tests
dotnet test
```

## Running the Database
Boot the PostgreSQL instance:
```bash
cd webuy-core
docker compose up -d
```
