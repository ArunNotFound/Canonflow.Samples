# OKF Catalog: public.audit
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| audit_id | uuid | False | None |
| table_name | text | False | None |
| record_id | uuid | False | None |
| action | text | False | CHECK ((action = ANY (ARRAY['INSERT'::text, 'UPDATE'::text, 'DELETE'::text]))) |
| timestamp | timestamp with time zone | False | None |
| actor_id | uuid | True | None |
