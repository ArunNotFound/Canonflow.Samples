# Wecar App (`wecar-core`)

The **Wecar App** is a CanonFlow sample showcasing a WeChat-like messaging application backend. This sample demonstrates the ultimate state-of-the-art (SOTA) "Hybrid" approach, unifying Database-First design, Domain-Driven Design (via FsAssay), and CanonFlow's automatic FSA properties.

## Features & Highlights

1. **Database-First Schema (PostgreSQL)**
   - Tables: `users`, `user_profiles`, `groups`, `group_members`, and `messages`.
   - Rich `CHECK` constraints: 
     - Username length/regex validation.
     - Role and Status enumerations.
     - Complex cross-column constraints (e.g., a message must belong to EITHER a receiver OR a group, never both or neither).
     - Chronological timeline checks (e.g., `read_at >= sent_at`).

2. **CanonFlow FSA (Finite State Automaton) Integration**
   - The CanonFlow diagnostic engine parses the schema and automatically emits mathematically proven FsCheck Arbitraries (`Generators.fs`).
   - Generates Zod validators directly from PostgreSQL constraints for frontend type-safety.

3. **FsAssay Pattern (Primitive Obsession Prevention)**
   - Smart constructors implemented for domain models (`Username`, `PhoneNumber`, `MessageContent`) to ensure invalid states cannot exist in memory.
   - For example, passing `Double.NaN`, negatives, or wrongly formatted text is intercepted explicitly at creation time via `Result<T, Error>`.
   - Ensures strict behavioral constraints (e.g., a message cannot be marked as read before it was sent).

4. **Air-Tight Property-Based Testing (FsCheck & xUnit)**
   - Unifies CanonFlow's dynamically generated FSA Arbitraries and the explicit FsAssay domain logic.
   - The property test suite leverages FsCheck to fuzz-test the smart constructors, proving that invalid transitions and data are blocked 100% of the time at the type level.

## Project Structure

- `db/init/` - PostgreSQL initialization script (`01-schema.sql`).
- `src/Domain/WecarCore.Domain/` - F# Domain Logic showcasing the FsAssay pattern.
- `tests/wecar.Tests/` - xUnit & FsCheck property tests asserting domain safety.
- `client/` - TypeScript environment with Jest testing the CanonFlow auto-generated Zod validators.
- `scripts/` - Automation scripts for scaffolding tests and running the CanonFlow CLI.
- `output/` - CanonFlow-generated artifacts (Catalogs, PROOF.md, FsCheck Generators, Contexts).

## Running the Tests

To run the property tests (which dynamically fuzz the FsAssay constraints):

```bash
cd tests/wecar.Tests
dotnet test
```

To test the frontend validators using Jest:

```bash
cd client
npm test
```
