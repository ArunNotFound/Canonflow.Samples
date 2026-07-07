# OKF Catalog: public.claims
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| claim_id | uuid | False | None |
| bill_id | uuid | False | None |
| insurance_id | uuid | False | None |
| claimed_amount | numeric | False | CHECK (((approved_amount >= (0)::numeric) AND (approved_amount <= claimed_amount))), CHECK ((claimed_amount > (0)::numeric)) |
| approved_amount | numeric | True | CHECK (((approved_amount >= (0)::numeric) AND (approved_amount <= claimed_amount))) |
| status | text | False | CHECK ((status = ANY (ARRAY['SUBMITTED'::text, 'UNDER_REVIEW'::text, 'APPROVED'::text, 'PARTIAL_APPROVED'::text, 'REJECTED'::text]))) |
| submitted_date | timestamp with time zone | False | None |
