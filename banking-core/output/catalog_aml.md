# OKF Catalog: public.aml
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| aml_id | uuid | False | None |
| customer_id | uuid | False | None |
| screening_status | text | False | CHECK ((screening_status = ANY (ARRAY['CLEARED'::text, 'FLAGGED'::text, 'UNDER_REVIEW'::text]))) |
| last_screened | timestamp with time zone | False | None |
