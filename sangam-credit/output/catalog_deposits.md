# OKF Catalog: public.deposits
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| deposit_id | integer | False | None |
| member_id | integer | False | None |
| amount | numeric | False | CHECK (((amount)::numeric >= (500)::numeric)), CHECK ((amount >= (100)::numeric)) |
| opened_on | date | False | CHECK ((maturity_date > opened_on)) |
| maturity_date | date | False | CHECK ((maturity_date > opened_on)) |
| rate_pct | numeric | False | CHECK (((rate_pct >= 3.5) AND (rate_pct <= 9.25))) |
