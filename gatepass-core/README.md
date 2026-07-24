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

3. **FsAssay Pattern (Primitive Obsession Prevention)**
   - Smart constructors implemented for domain models (`PhoneNumber`, `VehicleRegistration`, `PassDuration`) to block invalid states directly at the type level.
   - Includes regex matching for Indian standard vehicle registration patterns (e.g., `KA 01 AB 1234`) and duration bounds (1 to 72 hours).
   - Ensures strict state transitions (e.g., `Pending` -> `Approved` -> `Entered` -> `Exited`).

4. **Property-Based Testing (FsCheck & xUnit)**
   - Mathematical proofs leveraging generated FSA Arbitraries and the FsAssay smart constructors to dynamically bombard constraints with edge-case test data (like `NaN`, `Infinity`, boundary hours, and invalid patterns).

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
