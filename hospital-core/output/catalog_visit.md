# OKF Catalog: public.visit
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| visit_id | uuid | False | None |
| patient_id | uuid | False | None |
| doctor_id | uuid | False | None |
| visit_time | timestamp with time zone | False | None |
| type | text | False | CHECK ((type = ANY (ARRAY['OUTPATIENT'::text, 'INPATIENT'::text, 'EMERGENCY'::text]))) |
| status | text | False | CHECK ((status = ANY (ARRAY['SCHEDULED'::text, 'IN_PROGRESS'::text, 'COMPLETED'::text, 'CANCELLED'::text]))) |
