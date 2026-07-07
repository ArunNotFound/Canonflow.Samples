# OKF Catalog: public.loyalty
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| loyalty_id | uuid | False | None |
| passenger_id | uuid | False | None |
| tier | text | False | CHECK ((tier = ANY (ARRAY['BLUE'::text, 'SILVER'::text, 'GOLD'::text, 'PLATINUM'::text]))) |
| points | integer | False | CHECK ((points >= 0)) |
