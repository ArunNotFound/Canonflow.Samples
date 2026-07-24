# OKF Catalog: public.user_profiles
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| user_id | uuid | False | None |
| bio | character varying | True | None |
| avatar_url | character varying | True | None |
| privacy_setting | character varying | False | CHECK (((privacy_setting)::text = ANY ((ARRAY['EVERYONE'::character varying, 'CONTACTS'::character varying, 'NOBODY'::character varying])::text[]))) |
