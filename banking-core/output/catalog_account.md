# OKF Catalog: public.account
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| account_id | uuid | False | None |
| customer_id | uuid | False | None |
| branch_id | uuid | False | None |
| currency | text | False | None |
| account_type | text | False | CHECK ((account_type = ANY (ARRAY['SAVINGS'::text, 'CURRENT'::text, 'LOAN'::text, 'FD'::text]))) |
| balance | numeric | False | None |
| status | text | False | CHECK ((status = ANY (ARRAY['OPEN'::text, 'CLOSED'::text, 'FROZEN'::text]))) |
