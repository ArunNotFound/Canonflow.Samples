# OKF Catalog: public.seat
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| aircraft_id | uuid | False | None |
| seat_number | text | False | None |
| class | text | False | CHECK ((class = ANY (ARRAY['ECONOMY'::text, 'PREMIUM'::text, 'BUSINESS'::text, 'FIRST'::text]))) |
