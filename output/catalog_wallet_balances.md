# OKF Catalog: public.wallet_balances
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| wallet_id | uuid | False | None |
| available_balance | numeric | False | CHECK ((available_balance >= (0)::numeric)) |
| locked_balance | numeric | False | CHECK ((locked_balance >= (0)::numeric)) |
| updated_at | timestamp with time zone | False | None |
