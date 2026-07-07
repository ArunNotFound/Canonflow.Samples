# OKF Catalog: public.customer
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| customer_id | uuid | False | None |
| cif_number | text | False | CHECK ((length(cif_number) = 8)) |
| full_name | text | False | None |
| date_of_birth | date | False | CHECK ((date_of_birth < CURRENT_DATE)) |
| risk_rating | text | False | CHECK ((risk_rating = ANY (ARRAY['LOW'::text, 'MEDIUM'::text, 'HIGH'::text]))) |
| status | text | False | CHECK ((status = ANY (ARRAY['ACTIVE'::text, 'INACTIVE'::text, 'DORMANT'::text]))) |
