# OKF Catalog: public.loans
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| loan_id | integer | False | None |
| member_id | integer | False | None |
| principal | numeric | False | CHECK (((principal >= (1000)::numeric) AND (principal <= (500000)::numeric))) |
| tenure_months | integer | False | CHECK (((tenure_months >= 3) AND (tenure_months <= 84))) |
| interest_pct | numeric | False | CHECK ((interest_pct > (0)::numeric)), CHECK ((interest_pct <= (24)::numeric)), CHECK ((interest_pct <= (18)::numeric)) |
