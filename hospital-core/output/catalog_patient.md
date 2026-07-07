# OKF Catalog: public.patient
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| patient_id | uuid | False | None |
| first_name | text | False | None |
| last_name | text | False | None |
| date_of_birth | date | False | CHECK ((date_of_birth <= CURRENT_DATE)) |
| gender | text | False | CHECK ((gender = ANY (ARRAY['M'::text, 'F'::text, 'O'::text]))) |
| blood_group | text | True | CHECK ((blood_group = ANY (ARRAY['A+'::text, 'A-'::text, 'B+'::text, 'B-'::text, 'O+'::text, 'O-'::text, 'AB+'::text, 'AB-'::text]))) |
| status | text | False | CHECK ((status = ANY (ARRAY['REGISTERED'::text, 'ADMITTED'::text, 'DISCHARGED'::text, 'DECEASED'::text]))) |
