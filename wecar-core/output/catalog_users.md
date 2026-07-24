# OKF Catalog: public.users
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| user_id | uuid | False | None |
| username | character varying | False | CHECK (((length((username)::text) >= 3) AND (length((username)::text) <= 50))), CHECK (((username)::text ~ '^[a-zA-Z0-9_]+$'::text)) |
| display_name | character varying | False | None |
| phone_number | character varying | False | CHECK (((length((phone_number)::text) >= 10) AND (length((phone_number)::text) <= 15))) |
| status | character varying | False | CHECK (((status)::text = ANY ((ARRAY['ACTIVE'::character varying, 'INACTIVE'::character varying, 'BANNED'::character varying, 'DELETED'::character varying])::text[]))) |
| created_at | timestamp with time zone | True | None |
