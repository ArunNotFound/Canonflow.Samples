# OKF Catalog: public.loan
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| loan_id | uuid | False | None |
| account_id | uuid | False | None |
| principal_amount | numeric | False | CHECK ((principal_amount > (0)::numeric)) |
| outstanding_balance | numeric | False | CHECK ((outstanding_balance >= (0)::numeric)) |
| interest_rate | numeric | False | CHECK ((interest_rate >= (0)::numeric)) |
| status | text | False | CHECK ((status = ANY (ARRAY['DISBURSED'::text, 'CLOSED'::text, 'DEFAULTED'::text]))) |
