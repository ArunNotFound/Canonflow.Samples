# OKF Catalog: public.bookings
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| id | uuid | False | None |
| resource_id | uuid | False | None |
| user_email | character varying | False | None |
| start_time | timestamp with time zone | False | CHECK ((end_time > start_time)) |
| end_time | timestamp with time zone | False | CHECK ((end_time > start_time)) |
| attendee_count | integer | False | CHECK ((attendee_count > 0)) |
| status | character varying | False | CHECK (((status)::text = ANY ((ARRAY['PENDING'::character varying, 'CONFIRMED'::character varying, 'CANCELLED'::character varying, 'COMPLETED'::character varying])::text[]))) |
| total_cost | numeric | False | CHECK ((total_cost >= 0.00)) |
