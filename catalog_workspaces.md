# OKF Catalog: public.workspaces
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| id | uuid | False | None |
| name | character varying | False | CHECK (((plan_tier)::text = ANY ((ARRAY['FREE'::character varying, 'PRO'::character varying, 'ENTERPRISE'::character varying])::text[]))) |
| plan_tier | character varying | False | CHECK (((plan_tier)::text = ANY ((ARRAY['FREE'::character varying, 'PRO'::character varying, 'ENTERPRISE'::character varying])::text[]))) |
| created_at | timestamp with time zone | True | None |
