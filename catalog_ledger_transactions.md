# OKF Catalog: public.ledger_transactions
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| transaction_id | uuid | False | None |
| wallet_id | uuid | False | None |
| amount | numeric | False | CHECK ((amount > (0)::numeric)) |
| currency | character varying | False | CHECK ((amount > (0)::numeric)) |
| direction | character varying | False | CHECK (((direction)::text = ANY ((ARRAY['CREDIT'::character varying, 'DEBIT'::character varying])::text[]))) |
| status | character varying | False | CHECK (((status)::text = ANY ((ARRAY['CREATED'::character varying, 'PENDING'::character varying, 'COMPLETED'::character varying, 'FAILED'::character varying, 'REVERSED'::character varying])::text[]))) |
| reference_id | character varying | False | None |
| created_at | timestamp with time zone | False | CHECK (((direction)::text = ANY ((ARRAY['CREDIT'::character varying, 'DEBIT'::character varying])::text[]))) |
