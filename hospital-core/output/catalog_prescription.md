# OKF Catalog: public.prescription
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| prescription_id | uuid | False | None |
| visit_id | uuid | False | None |
| doctor_id | uuid | False | None |
| medication_name | text | False | None |
| dosage | text | False | None |
| frequency | text | False | None |
| duration_days | integer | False | CHECK (((duration_days > 0) AND (duration_days <= 365))) |
| is_controlled_substance | boolean | False | None |
| status | text | False | CHECK ((status = ANY (ARRAY['ACTIVE'::text, 'FILLED'::text, 'CANCELLED'::text, 'EXPIRED'::text]))) |
