# Layam Academy — CanonFlow Dogfood App

A complete line-of-business domain — a Carnatic music school — designed so that
**every table exercises a specific CanonFlow capability**, including two
deliberately planted specimens (marked ⚠️) that must produce diagnostics, not
silence. If CanonFlow digests this schema cleanly, honestly reports what it
cannot digest, and catches both specimens, the dogfood passes.

## The domain
Students enroll with gurus into batches (levels 1–8, Carnatic tradition);
exams are graded; fees are paid; scholarships have rules. Real LoB shape:
7 tables, 11 FKs, composite keys, 19 CHECK constraints across every
difficulty class.

## Constraint inventory → what it exercises

| Constraint | CanonFlow feature under test |
|---|---|
| `students.age CHECK (age >= 5 AND age <= 90)` | FParsec AND parsing → lattice `And` → `Refined<int, AgeTag>` with two-bound Range |
| `batches.fee_monthly CHECK (fee_monthly >= 500 AND fee_monthly <= 15000)` | decimal bounds (no float loss) + the Fable TS form rejecting ₹499 and ₹15001 client-side |
| `batches.capacity CHECK (capacity > 0 AND capacity <= 12)` | subsumption fodder: query `capacity > 0 AND capacity > 4` must simplify (ADR-015) |
| `enrollments (student_id, batch_id) PRIMARY KEY` | composite key (T7) |
| `enrollments.discount_pct CHECK (discount_pct >= 0 AND discount_pct <= 25)` | agreement test: F# and generated TS must reject 26 identically |
| `exams.marks CHECK (marks >= 0 AND marks <= 100)` | the classic — README "oh." moment |
| `exams CHECK (theory_marks + practical_marks <= 100)` | **arithmetic → unparseable → must surface as `Leaf(Opaque …)`, classified loss, never silently dropped** |
| `gurus.specialization CHECK (specialization IN ('vocal','violin','veena','mridangam','flute'))` | **IN-list → Opaque today** (honest loss; future parser increment) |
| `payments.method CHECK (method IN (…))` + `amount > 0` | mixed liftable/Opaque in one table — fidelity summary must show both |
| ⚠️ `scholarships`: `CHECK (min_attendance_pct > 90)` **and** `CHECK (min_attendance_pct < 75)` | **planted contradiction** — the semantic optimizer must collapse the conjunction to `False` and the CLI must name both constraints in a diagnostic. If this merges to code silently, ADR-015 has failed |
| ⚠️ `03-drift.sql` raises the fee cap to 20000 directly in the DB | **planted drift** — the Drift engine must report exactly one `DriftViolation` on `batches.fee_monthly`, report-only (ADR-002) |
| OpenSearch emission of `students` | fidelity must be `Approximate` with the reason string (FKs/defaults dropped) — classified loss at the emitter |

## The dogfood run

```bash
docker compose up -d                     # postgres:16 with schema + seed
./scripts/dogfood.sh                     # steps below, scripted
```

1. `canonflow introspect --pg $CONN --out generated/` →
   read `generated/Domain.fs`: expect `Refined` types with provenance
   comments (`derived-from: exams.marks via CHECK (marks >= 0 AND ...)`) and
   two honest `Opaque` leaves (arithmetic, IN-list). **Count them: exactly 3
   Opaque across the schema — more means parser regression, fewer means
   silent loss.**
2. `canonflow diagnose` → expect ONE contradiction diagnostic naming
   `scholarships.min_attendance_pct` and both CHECK clauses. ⚠️ specimen 1.
3. `canonflow contracts --ts client/` → open `client/validate.ts`; run
   `node scripts/agreement-spot.js` — ₹499 fee, age 4, discount 26, marks 101
   all rejected with the same verdicts F# gives.
4. `psql -f db/init/03-drift.sql` then `canonflow drift --expected generated/`
   → exactly one violation, `batches.fee_monthly`, advisory only. ⚠️ specimen 2.
5. `canonflow emit --target opensearch` → fidelity summary shows
   `Approximate` with reasons; nothing silent.

## Pass criteria (falsifiable, per the constitution)
- 16 constraints lifted to proofs, 3 Opaque, 0 silently dropped
- 1 contradiction diagnostic, 1 drift violation — no more, no fewer
- TS/F# agreement on the four boundary probes
- The whole run under 30 minutes for a stranger (this doubles as the
  Phase 2 gate rehearsal)

லயம் — rhythm. A school runs on constraints the way a tala runs on beats;
CanonFlow's job is to prove it never drops one.
