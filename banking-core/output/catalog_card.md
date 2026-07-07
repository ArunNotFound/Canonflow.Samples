# OKF Catalog: public.card
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| card_id | uuid | False | None |
| account_id | uuid | False | None |
| card_number | text | False | CHECK ((length(card_number) >= 16)) |
| card_type | text | False | CHECK ((card_type = ANY (ARRAY['DEBIT'::text, 'CREDIT'::text, 'PREPAID'::text]))) |
| status | text | False | CHECK ((status = ANY (ARRAY['ACTIVE'::text, 'BLOCKED'::text, 'EXPIRED'::text]))) |
