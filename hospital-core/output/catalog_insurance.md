# OKF Catalog: public.insurance
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| insurance_id | uuid | False | None |
| patient_id | uuid | False | None |
| provider_name | text | False | None |
| policy_number | text | False | None |
| coverage_limit | numeric | False | CHECK ((coverage_limit >= (0)::numeric)) |
| copay_percentage | integer | False | CHECK (((copay_percentage >= 0) AND (copay_percentage <= 100))) |
| valid_until | date | False | None |
| status | text | False | CHECK ((status = ANY (ARRAY['ACTIVE'::text, 'EXPIRED'::text, 'SUSPENDED'::text]))) |
