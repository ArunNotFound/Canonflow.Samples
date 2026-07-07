# OKF Catalog: public.branch
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| branch_id | uuid | False | None |
| branch_code | text | False | CHECK ((length(branch_code) = 4)) |
| name | text | False | None |
| status | text | False | CHECK ((status = ANY (ARRAY['ACTIVE'::text, 'CLOSED'::text, 'SUSPENDED'::text]))) |
