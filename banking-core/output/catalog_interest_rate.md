# OKF Catalog: public.interest_rate
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| rate_id | uuid | False | None |
| account_type | text | False | None |
| rate_percentage | numeric | False | CHECK (((rate_percentage >= (0)::numeric) AND (rate_percentage <= (100)::numeric))) |
| effective_date | timestamp with time zone | False | None |
