# OKF Catalog: public.users
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| user_id | uuid | False | None |
| role | character varying | False | CHECK (((role)::text = ANY ((ARRAY['CUSTOMER'::character varying, 'PROFESSIONAL'::character varying])::text[]))) |
| full_name | character varying | False | CHECK (((length((full_name)::text) >= 2) AND (length((full_name)::text) <= 100))) |
| phone_number | character varying | False | CHECK (((length((phone_number)::text) >= 10) AND (length((phone_number)::text) <= 15))) |
| email | character varying | True | None |
| status | character varying | False | CHECK (((status)::text = ANY ((ARRAY['ACTIVE'::character varying, 'INACTIVE'::character varying, 'BANNED'::character varying])::text[]))) |
| created_at | timestamp with time zone | True | None |
