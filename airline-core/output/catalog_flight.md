# OKF Catalog: public.flight
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| flight_id | uuid | False | None |
| flight_number | text | False | None |
| aircraft_id | uuid | False | None |
| departure_time | timestamp with time zone | False | CHECK ((arrival_time > departure_time)) |
| arrival_time | timestamp with time zone | False | CHECK ((arrival_time > departure_time)) |
| origin | text | False | CHECK ((length(origin) = 3)), CHECK ((origin <> destination)) |
| destination | text | False | CHECK ((length(destination) = 3)), CHECK ((origin <> destination)) |
| status | text | False | CHECK ((status = ANY (ARRAY['SCHEDULED'::text, 'BOARDING'::text, 'DEPARTED'::text, 'ARRIVED'::text, 'CANCELLED'::text]))) |
