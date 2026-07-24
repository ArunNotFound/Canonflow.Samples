# OKF Catalog: public.residents
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| resident_id | uuid | False | None |
| full_name | character varying | False | None |
| unit_number | character varying | False | CHECK ((length((unit_number)::text) > 0)) |
| phone | character varying | False | CHECK (((length((phone)::text) >= 10) AND (length((phone)::text) <= 15))) |
| email | character varying | True | None |
| is_active | boolean | False | None |
