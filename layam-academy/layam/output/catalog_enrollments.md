# OKF Catalog: public.enrollments
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| student_id | integer | False | None |
| batch_id | integer | False | None |
| discount_pct | numeric | False | CHECK (((discount_pct >= (0)::numeric) AND (discount_pct <= (25)::numeric))) |
