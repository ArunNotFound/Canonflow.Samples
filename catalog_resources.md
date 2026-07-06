# OKF Catalog: public.resources
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| id | uuid | False | None |
| workspace_id | uuid | False | None |
| name | character varying | False | CHECK (((capacity >= 1) AND (capacity <= 1000))) |
| capacity | integer | False | CHECK (((capacity >= 1) AND (capacity <= 1000))) |
| hourly_rate | numeric | False | CHECK ((hourly_rate >= 0.00)) |
