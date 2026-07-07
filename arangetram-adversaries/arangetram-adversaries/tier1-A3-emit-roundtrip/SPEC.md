# A3 · The Greenfield Gauntlet  [LOAD-BEARING — the untested half of the law]

## What it attacks
Every specimen to date sieges introspect. The constitution is
`introspect(emit(domain)) ≅ domain`. emit has never faced an adversary.

## The adversary
A hand-built F# domain (Domain.fs in this folder is a starting sketch) with
shapes that DON'T map cleanly to SQL, forcing honest outward classification:
- recursive type (org chart: Employee has manager: Employee option)
- deeply optional records (option of option, via wrapper)
- a DU with 5 cases that no single SQL column represents
- a Refined type whose predicate is an OR (must emit as CHECK with OR, or
  classify Approximate)
- a decimal with scale exceeding NUMERIC's practical range
- a unit-of-measure phantom that has no SQL correlate (must be Unrepresentable)

## Pass criteria (falsifiable)
- emit produces DDL that DEPLOYS to a fresh Postgres container.
- Each non-representable shape yields a classified FieldClass
  (Widened/Narrowed/Unrepresentable) WITH a reason — never a silent drop,
  never a crash.
- Round-trip: emit(domain) → deploy → introspect → `≅ domain` with the loss
  ledger matching the predicted classifications exactly.
- Determinism (A1) holds on emit output too.
