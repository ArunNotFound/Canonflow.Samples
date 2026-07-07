# OKF Catalog: public.passenger
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| passenger_id | uuid | False | None |
| passport_number | text | False | None |
| full_name | text | False | None |
| dob | date | False | CHECK ((dob < CURRENT_DATE)) |
| status | text | False | CHECK ((status = ANY (ARRAY['ACTIVE'::text, 'BANNED'::text]))) |
