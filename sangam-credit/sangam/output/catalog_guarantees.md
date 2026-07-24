# OKF Catalog: public.guarantees
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| loan_id | integer | False | None |
| guarantor_id | integer | False | None |
| guarantor_share_pct | numeric | True | CHECK ((guarantor_share_pct >= (10)::numeric)) |
