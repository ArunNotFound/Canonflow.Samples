# OKF Catalog: public.payment
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| payment_id | uuid | False | None |
| booking_id | uuid | False | None |
| amount | numeric | False | CHECK ((amount > (0)::numeric)) |
| currency | text | False | CHECK ((length(currency) = 3)) |
| status | text | False | CHECK ((status = ANY (ARRAY['PENDING'::text, 'AUTHORIZED'::text, 'CAPTURED'::text, 'REFUNDED'::text, 'FAILED'::text]))) |
| timestamp | timestamp with time zone | False | None |
