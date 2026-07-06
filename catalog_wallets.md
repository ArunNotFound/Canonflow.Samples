# OKF Catalog: public.wallets
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| wallet_id | uuid | False | None |
| customer_id | uuid | False | None |
| currency | character varying | False | CHECK (((status)::text = ANY ((ARRAY['ACTIVE'::character varying, 'SUSPENDED'::character varying, 'CLOSED'::character varying])::text[]))) |
| status | character varying | False | CHECK (((status)::text = ANY ((ARRAY['ACTIVE'::character varying, 'SUSPENDED'::character varying, 'CLOSED'::character varying])::text[]))) |
| created_at | timestamp with time zone | False | None |
