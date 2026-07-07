# OKF Catalog: public.doctor
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| doctor_id | uuid | False | None |
| license_number | text | False | None |
| full_name | text | False | None |
| specialty | text | False | CHECK ((specialty = ANY (ARRAY['GENERAL'::text, 'CARDIOLOGY'::text, 'NEUROLOGY'::text, 'PEDIATRICS'::text, 'ONCOLOGY'::text, 'SURGERY'::text]))) |
| status | text | False | CHECK ((status = ANY (ARRAY['ACTIVE'::text, 'ON_LEAVE'::text, 'RETIRED'::text, 'TERMINATED'::text]))) |
