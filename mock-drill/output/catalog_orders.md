# OKF Catalog: public.orders
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| order_id | uuid | False | None |
| customer_id | uuid | False | None |
| amount | numeric | False | CHECK ((amount > (0)::numeric)) |
| currency | character | False | CHECK ((currency = ANY (ARRAY['INR'::bpchar, 'USD'::bpchar, 'EUR'::bpchar]))) |
| order_status | character varying | False | CHECK (((order_status)::text = ANY ((ARRAY['PLACED'::character varying, 'PAID'::character varying, 'CANCELLED'::character varying])::text[]))) |
| created_at | timestamp without time zone | False | None |
