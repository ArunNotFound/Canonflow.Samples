# OKF Catalog: public.booking
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| booking_id | uuid | False | None |
| passenger_id | uuid | False | None |
| flight_id | uuid | False | None |
| aircraft_id | uuid | False | None |
| seat_number | text | False | None |
| booking_time | timestamp with time zone | False | None |
| status | text | False | CHECK ((status = ANY (ARRAY['HOLD'::text, 'CONFIRMED'::text, 'CANCELLED'::text]))) |
