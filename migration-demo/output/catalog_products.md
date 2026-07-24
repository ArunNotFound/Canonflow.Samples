# OKF Catalog: public.products
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| product_id | uuid | False | None |
| name | text | False | None |
| price | numeric | False | CHECK ((price >= (0)::numeric)) |
