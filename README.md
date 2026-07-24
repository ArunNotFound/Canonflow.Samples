# CanonFlow Enterprise Samples

This repository contains battle-tested architectural blueprints demonstrating the power of **CanonFlow**—the bridge between Database Nouns (Structural Constraints) and Application Verbs (Domain-Driven Design).

## The CanonFlow Golden Rule
1. **The Database (Nouns)**: If a business rule defines a structural limit (e.g., `amount > 0`) or an interconnected reality (e.g., a booked seat must exist on the specific aircraft), it belongs in the Database as a `CHECK` constraint.
2. **The Application (Verbs)**: If a business rule defines a process, transition, or external side-effect (e.g., checking market hours, adjudicating a claim), it belongs in the Application Tier (F# DDD).
3. **The Bridge (CanonFlow)**: CanonFlow mathematically transpiles the DB Nouns into TypeScript validators and OpenMetadata catalogs. You write zero boilerplate to keep the UI, API, and DB in sync.

## The Samples

* [Kutcheri Season](kutcheri-season/README.md) - Demonstrates String Ranges, Regex patterns, and strict capacity modeling for classical music festivals.
* [Banking Core](banking-core/README.md) - Demonstrates F# Event Sourcing working in tandem with strict Ledger constraints.
* [Hospital Core](hospital-core/README.md) - Demonstrates massive schema inference and timeline boundaries (e.g. Lab results cannot be completed before they are ordered).
* [Airline Core](airline-core/README.md) - Demonstrates deep **Interconnected Overlap constraints** using Composite Foreign Keys.
* [Trading Core](trading-core/README.md) - Demonstrates the ultimate DB + DDD synergy, splitting structural bounds (DB) and historical bounds (F#).
* [Migration Engine Demo](migration-demo/README.md) - Demonstrates CanonFlow's diffing engine generating state-transition SQL.

### Running a Sample
Every sample includes a `docker-compose.yml` and a `dogfood.sh` script to instantly spin up the Postgres database, run CanonFlow, and emit the contracts.

## Validation & Property-Based Testing
To guarantee that the **Structural Constraints** defined in Postgres perfectly match the generated application contracts, we are currently rolling out comprehensive validation across all samples:

1. **FsCheck Property-Based Tests (F#)**: CanonFlow generates mathematical boundaries (Arbitraries) based directly on the SQL `CHECK` constraints (using the `--fscheck` flag). We run xUnit tests against these Arbitraries to prove they prevent invalid data.
2. **Jest Validation (TypeScript)**: CanonFlow generates Zod schemas for the client. We run Jest test suites to prove the frontend validation logic matches the backend.

### Implementation Status
* **Completed (F# & TypeScript)**: `mock-drill`, `kutcheri-season`, `banking-core`
* **Completed (F# Only)**: `hospital-core`, `airline-core`, `trading-core`
* **Pending / Custom Implementations**: `migration-demo`, `arangetram-adversaries`, `layam-academy`, `sangam-credit`

*Note: For the current progress checklist, see [list.md](list.md).*
