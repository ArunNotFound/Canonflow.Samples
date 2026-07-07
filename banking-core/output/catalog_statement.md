# OKF Catalog: public.statement
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| statement_id | uuid | False | None |
| account_id | uuid | False | None |
| period_start | date | False | CHECK ((period_end > period_start)) |
| period_end | date | False | CHECK ((period_end > period_start)) |
| opening_balance | numeric | False | None |
| closing_balance | numeric | False | None |
