# A1 · Emit Determinism  [LOAD-BEARING — ADR-009's entire premise]

## What it attacks
Regenerated output must be byte-identical for an unchanged input, and
minimally-diffed for a small change. If not, "codegen over type providers,
diffable in PRs" (ADR-009) is unverified and every agent PR reviewing
generated code drowns in noise.

## Why load-bearing, not edge
This is not about output quality — it is about whether the central
architectural decision pays the dividend it was chosen for. Nondeterminism
here invalidates the bet.

## The adversary
1. IDEMPOTENCE: introspect the same DB twice → `diff` must be empty. Common
   culprit: pg_catalog row order is not guaranteed — sort every harvested
   collection by a stable key (schema, table, ordinal, constraint name).
2. MINIMAL CHURN: add one column, regenerate → git diff touches exactly the
   affected type's region, not the whole file. (Fantomas formatting must be
   pinned to a version — formatter drift is a determinism leak.)
3. ORDER INDEPENDENCE: harvest a DB, shuffle the internal table order, emit →
   byte-identical output. Ordering must be canonical, not incidental.

## Pass criteria (falsifiable)
- 100 introspect pairs over gauntlet-generated schemas: zero non-empty diffs.
- One-column-add churn test: diff line count < 2x the column's own lines.
- CI job `determinism` green; Fantomas version pinned in the .fsproj.
