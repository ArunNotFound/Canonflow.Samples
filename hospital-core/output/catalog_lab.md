# OKF Catalog: public.lab
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| lab_id | uuid | False | None |
| visit_id | uuid | False | None |
| test_name | text | False | None |
| result_value | text | True | None |
| is_abnormal | boolean | True | None |
| ordered_time | timestamp with time zone | False | CHECK (((completed_time IS NULL) OR (completed_time >= ordered_time))) |
| completed_time | timestamp with time zone | True | CHECK (((completed_time IS NULL) OR (completed_time >= ordered_time))) |
| status | text | False | CHECK ((status = ANY (ARRAY['ORDERED'::text, 'IN_PROGRESS'::text, 'COMPLETED'::text, 'CANCELLED'::text]))) |
