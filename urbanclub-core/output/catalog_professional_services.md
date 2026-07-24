# OKF Catalog: public.professional_services
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| professional_id | uuid | False | None |
| service_id | uuid | False | None |
| experience_years | integer | False | CHECK (((experience_years >= 0) AND (experience_years <= 50))) |
