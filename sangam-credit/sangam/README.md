# Sangam Credit Cooperative — CanonFlow Dogfood II

Layam Academy tested the happy AND-range path. Sangam attacks everything
Layam could not: **OR-logic, SQL three-valued NULL semantics, negative
numbers, DOMAIN types, quoted identifiers, subsumption chains, and drift in
both FieldClass directions.** Seven planted specimens, each numbered, each
with a falsifiable expected outcome. A cooperative credit society is the
domain because regulatory caps are CHECK constraints written by law.

## Specimen catalog

| # | Specimen | What must happen |
|---|---|---|
| S1 | **The NULL trap** — `guarantor_id INT NULL` + `CHECK (guarantor_share_pct >= 10)` on a NULLable column. In SQL's three-valued logic a CHECK **passes when the expression is NULL** — the DB admits rows Layam-style naive `Refined<decimal,_>` would reject. | The generated proof must be `Refined<option<decimal>>`-shaped (or the doc must state the NULL-admits semantics). If CanonFlow emits a non-optional proof here, it has *invented a constraint the database does not enforce* — the inverse of silent loss, equally forbidden. This is the star specimen. |
| S2 | **OR eligibility** — `CHECK (age >= 21 OR guardian_member_id IS NOT NULL)` | AND-only parsers die here. Expected today: `Or` parses, `IS NOT NULL` leg → Opaque *inside* the Or — partial lift with honest mixed fidelity, not whole-clause Opaque. |
| S3 | **Negative bounds** — `CHECK (ledger_adjustment >= -5000 AND ledger_adjustment <= 5000)` | Known parser gap (pDecimal has no sign). Expected today: Opaque. **This is a canary specimen**: when the parser learns signs, the Opaque count drops by one and the README must be updated — specimens version the parser. |
| S4 | **Subsumption chain** — three constraints on `loans.interest_pct`: table CHECK `> 0`, named CHECK `<= 24`, second named CHECK `<= 18` (the RBI-style tightening amendment) | ADR-015 must fold the conjunction to the single interval `(0, 18]` — and report that the `<= 24` constraint is *redundant*, as data. Redundancy detection is subsumption's second gift after contradiction. |
| S5 | **DOMAIN type** — `CREATE DOMAIN share_amount AS NUMERIC(10,2) CHECK (VALUE >= 100)` used by two tables | Introspection must chase the domain: both columns inherit the proof, provenance says `via DOMAIN share_amount`. Domains are where enterprises actually keep reusable constraints; missing them undercounts by whole families. |
| S6 | **Quoted identifier** — column `"riskGrade"` (CamelCase, quoted) with a CHECK | Harvest + codegen must survive case-sensitivity; generated F#/TS/Kotlin field names must round-trip it. Classic silent-breakage territory. |
| S7 | **Bidirectional drift** — `03-drift.sql` *loosens* the loan cap (Widened) and *tightens* the deposit minimum (Narrowed) in one script | Drift must report **two** violations with **opposite FieldClass directions** — proving the four-way taxonomy is live, not decorative. Narrowed is the dangerous one (existing rows may violate); the report must say so. |

Plus ambient coverage Layam lacks: date-window CHECK (`maturity_date > opened_on` — cross-column → Opaque, counted), BETWEEN syntax (→ Opaque today, second canary), a deferrable composite FK, and money at NUMERIC(12,2) scale.

## Expected counts (falsifiable, update when the parser grows)
- Lifted to proofs: 11 · Opaque: 6 (S2-leg, S3, BETWEEN, date-window, phone-length, IN-list) · Invented constraints: **0** (S1 audit)
- Diagnostics: 1 redundancy (S4) · 0 contradictions · Drift: exactly 2, opposite directions (S7)

## Run
`docker compose up -d && ./scripts/dogfood.sh` — same shape as Layam.
சங்கம் — the assembly. A cooperative runs on rules its members can inspect;
so does this schema.
