# OKF Catalog: public.group_members
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| group_id | uuid | False | None |
| user_id | uuid | False | None |
| role | character varying | False | CHECK (((role)::text = ANY ((ARRAY['ADMIN'::character varying, 'MODERATOR'::character varying, 'MEMBER'::character varying])::text[]))) |
| joined_at | timestamp with time zone | True | None |
