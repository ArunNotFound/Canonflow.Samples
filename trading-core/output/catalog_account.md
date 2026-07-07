# OKF Catalog: public.account
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| account_id | uuid | False | None |
| owner_name | text | False | None |
| cash_balance | numeric | False | CHECK ((cash_balance >= (0)::numeric)) |
