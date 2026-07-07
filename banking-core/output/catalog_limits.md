# OKF Catalog: public.limits
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| limit_id | uuid | False | None |
| account_id | uuid | False | None |
| daily_transfer_limit | numeric | False | CHECK ((daily_transfer_limit >= (0)::numeric)) |
| overdraft_limit | numeric | False | CHECK ((overdraft_limit >= (0)::numeric)) |
