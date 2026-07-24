# OKF Catalog: public.services
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| service_id | uuid | False | None |
| category | character varying | False | CHECK (((category)::text = ANY ((ARRAY['CLEANING'::character varying, 'PLUMBING'::character varying, 'ELECTRICAL'::character varying, 'SALON'::character varying, 'APPLIANCE_REPAIR'::character varying])::text[]))) |
| name | character varying | False | None |
| base_price | numeric | False | CHECK ((base_price >= 0.0)) |
| is_active | boolean | False | None |
