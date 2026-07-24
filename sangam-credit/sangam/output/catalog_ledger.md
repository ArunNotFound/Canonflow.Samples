# OKF Catalog: public.ledger
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| entry_id | integer | False | None |
| member_id | integer | False | None |
| entry_on | timestamp with time zone | False | None |
| ledger_adjustment | numeric | False | CHECK (((ledger_adjustment >= ('-5000'::integer)::numeric) AND (ledger_adjustment <= (5000)::numeric))) |
| method | character varying | False | CHECK (((method)::text = ANY ((ARRAY['cash'::character varying, 'neft'::character varying, 'upi'::character varying])::text[]))) |
