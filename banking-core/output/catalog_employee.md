# OKF Catalog: public.employee
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| employee_id | uuid | False | None |
| branch_id | uuid | False | None |
| name | text | False | None |
| role | text | False | CHECK ((role = ANY (ARRAY['TELLER'::text, 'MANAGER'::text, 'AUDITOR'::text, 'OFFICER'::text]))) |
