# Gatepass App (`gatepass-core`)

The **Gatepass App** is a CanonFlow sample that demonstrates building a robust backend for managing residential/visitor gate passes. This sample illustrates how to achieve an "air-tight" guardrail system using both Database-First schema constraints and Domain-Driven Design (DDD) principles.

## Features & Highlights

1. **Database-First Schema (PostgreSQL)**
   - Includes tables for `residents`, `visitors`, and `gatepasses`.
   - Incorporates complex `CHECK` constraints (e.g., status enums, ID proof types, phone number validations, and arrival/departure timeline logic).
   - Validated against logical contradictions using CanonFlow's diagnostic engine.

2. **CanonFlow FSA (Finite State Automaton) Integration**
   - Automatically emitted FsCheck Arbitraries directly from Postgres constraints.
   - Automatically transpiled Zod validators for the TypeScript client.

3. **FsAssay Pattern (Primitive Obsession Prevention) - A Game Changer!**
   - **The Problem with Primitives:** Passing around strings for `PhoneNumber` or `VehicleRegistration` allows invalid data (like `Double.NaN`, negatives, or wrongly formatted text) to infiltrate deep into the system before failing.
   - **The FsAssay Solution:** We explicitly revisited the `gatepass-core` domain to replace these primitives with FsAssay "Smart Constructors". By returning a `Result<T, Error>` explicitly during object creation, invalid states simply *cannot exist* in memory. 
   - **How it Helps:** 
      - **Zero Invalid State:** Functions like `GatepassBehavior.approve` now implicitly trust their inputs because `PhoneNumber`, `VehicleRegistration`, and `PassDuration` guarantee validity statically at compile time.
      - **Explicit Modeling:** `PassDuration` blocks bounds (`< 1 hour` or `> 72 hours`) alongside `NaN` and `Infinity`.
      - **Regex Validation:** Vehicle Registration strictly enforces standards (e.g., `KA 01 AB 1234`).
   - **State Machine Guardrails:** Ensures strict state transitions for a gatepass (e.g., `Pending` -> `Approved` -> `Entered` -> `Exited`).

4. **Property-Based Testing (FsCheck & xUnit)**
   - Mathematical proofs leveraging generated FSA Arbitraries and the FsAssay smart constructors to dynamically bombard constraints with edge-case test data (like `NaN`, `Infinity`, boundary hours, and invalid patterns).
   - **Proof of Concept:** Our test suite actively forces FsAssay type creation with random memory fuzzing, proving that our domain intercepts exactly 100% of illegal arguments immediately.

## Project Structure

- `db/init/` - Contains the PostgreSQL initialization script (`01-schema.sql`).
- `src/Domain/GatepassCore.Domain/` - F# Domain Logic showcasing the FsAssay pattern.
- `tests/gatepass.Tests/` - xUnit & FsCheck property tests asserting domain safety.
- `client/` - TypeScript environment with Jest testing the CanonFlow auto-generated Zod validators.
- `scripts/` - Automation scripts for scaffolding tests and running the CanonFlow CLI.
- `output/` - CanonFlow-generated artifacts (Catalogs, PROOF.md, FsCheck Generators, Contexts).

## Running the Tests

To run the property tests, navigate to the test directory and use the `dotnet test` command:

```bash
cd tests/gatepass.Tests
dotnet test
```

To test the frontend validators using Jest:

```bash
cd client
npm test
```
