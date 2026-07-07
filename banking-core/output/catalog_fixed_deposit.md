# OKF Catalog: public.fixed_deposit
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| fd_id | uuid | False | None |
| account_id | uuid | False | None |
| principal | numeric | False | CHECK ((principal > (0)::numeric)) |
| maturity_date | date | False | None |
| status | text | False | CHECK ((status = ANY (ARRAY['ACTIVE'::text, 'MATURED'::text, 'BROKEN'::text]))) |
