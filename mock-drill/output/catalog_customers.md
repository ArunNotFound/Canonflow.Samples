# OKF Catalog: public.customers
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| customer_id | uuid | False | None |
| email | character varying | False | None |
| age | integer | False | CHECK (((age >= 18) AND (age < 120))) |
| status | character varying | False | CHECK (((status)::text = ANY ((ARRAY['ACTIVE'::character varying, 'SUSPENDED'::character varying, 'CLOSED'::character varying])::text[]))) |
| credit_limit | numeric | False | CHECK (((credit_limit >= (0)::numeric) AND (credit_limit <= (1000000)::numeric))) |
| created_at | timestamp without time zone | False | None |
