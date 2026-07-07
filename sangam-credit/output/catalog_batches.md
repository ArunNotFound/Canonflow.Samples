# OKF Catalog: public.batches
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| batch_id | integer | False | None |
| guru_id | integer | False | None |
| raga_focus | character varying | True | None |
| level | integer | False | CHECK (((level >= 1) AND (level <= 8))) |
| capacity | integer | False | CHECK (((capacity > 0) AND (capacity <= 12))) |
| fee_monthly | numeric | False | CHECK (((fee_monthly >= (500)::numeric) AND (fee_monthly <= (15000)::numeric))) |
