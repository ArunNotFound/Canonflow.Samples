# OKF Catalog: public.orders
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| order_id | uuid | False | None |
| user_id | uuid | False | None |
| total_amount | numeric | False | CHECK ((total_amount >= (0)::numeric)) |
| discount | numeric | False | CHECK ((discount >= (0)::numeric)) |
