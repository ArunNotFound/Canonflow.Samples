# CanonFlow Layer Map

This map defines the architectural boundary for each entity in the system.

## Layer 1: DB-Enforced Structural Truth
These entities model pure structural nouns. Their constraints are enforced directly by Postgres CHECK constraints.
- **bookings**
- **professional_services**
- **services**
- **users**

## Layer 2: App-Enforced Business Behavior
These entities model workflow and state-transition logic (Verbs). Their rules live in the application layer.

## Layer 3: Unenforced Master Data
These entities map to external truths (e.g. EDI SKUs) where the DB cannot enforce correctness.
*(Identified via configuration or lineage declarations)*
