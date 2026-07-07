# OKF Catalog: public.charges
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| charge_id | uuid | False | None |
| txn_id | uuid | False | None |
| charge_type | text | False | CHECK ((charge_type = ANY (ARRAY['FEE'::text, 'TAX'::text, 'PENALTY'::text]))) |
| amount | numeric | False | CHECK ((amount > (0)::numeric)) |
