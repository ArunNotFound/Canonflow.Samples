# OKF Catalog: public.ledger
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| ledger_id | uuid | False | None |
| txn_id | uuid | False | None |
| gl_account | text | False | None |
| entry_type | text | False | CHECK ((entry_type = ANY (ARRAY['DR'::text, 'CR'::text]))) |
| amount | numeric | False | CHECK ((amount > (0)::numeric)) |
