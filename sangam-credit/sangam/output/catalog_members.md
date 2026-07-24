# OKF Catalog: public.members
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| member_id | integer | False | None |
| full_name | character varying | False | None |
| phone | character varying | False | CHECK ((length((phone)::text) = 10)) |
| age | integer | False | CHECK (((age >= 21) OR (guardian_member_id IS NOT NULL))) |
| guardian_member_id | integer | True | CHECK (((age >= 21) OR (guardian_member_id IS NOT NULL))) |
| share_balance | numeric | False | CHECK ((share_balance >= (100)::numeric)) |
| riskGrade | character varying | False | CHECK (((("riskGrade")::text >= 'A'::text) AND (("riskGrade")::text <= 'E'::text))) |
