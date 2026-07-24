# OKF Catalog: public.users
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| user_id | uuid | False | None |
| email | text | False | None |
| status | text | False | CHECK ((status = ANY (ARRAY['ACTIVE'::text, 'BANNED'::text]))) |
| created_at | timestamp with time zone | False | None |
