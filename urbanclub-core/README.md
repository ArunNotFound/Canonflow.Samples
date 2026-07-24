# UrbanClub App (`urbanclub-core`)

The **UrbanClub App** is a CanonFlow sample showcasing a robust backend for a home services platform (like Urban Company). This sample demonstrates the SOTA "Hybrid" approach, seamlessly combining Database-First validation, CanonFlow's automatic FSA properties, and Domain-Driven Design (FsAssay).

## Features & Highlights

1. **Database-First Schema (PostgreSQL)**
   - Tables: `users`, `services`, `professional_services`, and `bookings`.
   - Rich `CHECK` constraints: 
     - User roles (`CUSTOMER`, `PROFESSIONAL`) and statuses.
     - Service category enumerations and strict non-negative pricing checks.
     - Chronological timeline checks (e.g., `completed_at >= scheduled_time`).
     - Validation of Professional Experience Years (0 to 50).

2. **CanonFlow FSA (Finite State Automaton) Integration**
   - Parses the PostgreSQL schema and automatically emits mathematically proven FsCheck Arbitraries (`Generators.fs`).
   - Generates Zod validators directly from PostgreSQL constraints for frontend type-safety.

3. **FsAssay Pattern (Primitive Obsession Prevention)**
   - Smart constructors implemented for domain models (`FullName`, `PhoneNumber`, `MoneyAmount`) to ensure invalid states never exist in memory.
   - For example, passing `Double.NaN`, negative money amounts, or out-of-bounds name lengths is intercepted explicitly at creation time via `Result<T, Error>`.
   - Strict behavioral constraints: a booking cannot be marked as completed with a timestamp prior to its scheduled time.

4. **Air-Tight Property-Based Testing (FsCheck & xUnit)**
   - The test suite leverages FsCheck to fuzz-test the smart constructors, dynamically bombing them with edge-case test data. This proves that invalid transitions and illegal data states are blocked 100% of the time at the type level.

## Project Structure

- `db/init/` - PostgreSQL initialization script (`01-schema.sql`).
- `src/Domain/UrbanclubCore.Domain/` - F# Domain Logic showcasing the FsAssay pattern.
- `tests/urbanclub.Tests/` - xUnit & FsCheck property tests asserting domain safety.
- `client/` - TypeScript environment with Jest testing the CanonFlow auto-generated Zod validators.
- `scripts/` - Automation scripts for scaffolding tests and running the CanonFlow CLI.
- `output/` - CanonFlow-generated artifacts (Catalogs, PROOF.md, FsCheck Generators, Contexts).

## Running the Tests

To run the property tests:

```bash
cd tests/urbanclub.Tests
dotnet test
```

To test the frontend validators using Jest:

```bash
cd client
npm test
```
