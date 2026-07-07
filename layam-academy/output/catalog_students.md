# OKF Catalog: public.students
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| student_id | integer | False | None |
| full_name | character varying | False | None |
| email | character varying | False | None |
| phone | character varying | False | CHECK ((length((phone)::text) = 10)) |
| age | integer | False | CHECK (((age >= 5) AND (age <= 90))) |
| enrolled_on | date | False | None |
