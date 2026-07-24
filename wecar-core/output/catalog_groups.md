# OKF Catalog: public.groups
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| group_id | uuid | False | None |
| name | character varying | False | CHECK (((length((name)::text) >= 1) AND (length((name)::text) <= 100))) |
| description | character varying | True | None |
| created_by | uuid | False | None |
| created_at | timestamp with time zone | True | None |
