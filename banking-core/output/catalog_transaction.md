# OKF Catalog: public.transaction
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| txn_id | uuid | False | None |
| account_id | uuid | False | None |
| amount | numeric | False | CHECK ((amount <> (0)::numeric)) |
| txn_type | text | False | CHECK ((txn_type = ANY (ARRAY['CREDIT'::text, 'DEBIT'::text]))) |
| currency | text | False | None |
| timestamp | timestamp with time zone | False | None |
| reference | text | False | None |
