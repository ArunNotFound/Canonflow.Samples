# OKF Catalog: public.exchange_rate
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| rate_id | uuid | False | None |
| base_currency | text | False | None |
| target_currency | text | False | None |
| rate | numeric | False | CHECK ((rate > (0)::numeric)) |
| effective_date | timestamp with time zone | False | None |
