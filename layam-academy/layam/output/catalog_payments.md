# OKF Catalog: public.payments
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| payment_id | integer | False | None |
| student_id | integer | False | None |
| batch_id | integer | False | None |
| amount | numeric | False | CHECK ((amount > (0)::numeric)) |
| method | character varying | False | CHECK (((method)::text = ANY ((ARRAY['upi'::character varying, 'card'::character varying, 'cash'::character varying])::text[]))) |
| paid_on | timestamp with time zone | False | None |
