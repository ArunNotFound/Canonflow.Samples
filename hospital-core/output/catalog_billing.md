# OKF Catalog: public.billing
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| bill_id | uuid | False | None |
| visit_id | uuid | False | None |
| total_amount | numeric | False | CHECK ((total_amount = (patient_responsibility + insurance_responsibility))), CHECK ((total_amount >= (0)::numeric)) |
| patient_responsibility | numeric | False | CHECK ((patient_responsibility >= (0)::numeric)), CHECK ((total_amount = (patient_responsibility + insurance_responsibility))) |
| insurance_responsibility | numeric | False | CHECK ((insurance_responsibility >= (0)::numeric)), CHECK ((total_amount = (patient_responsibility + insurance_responsibility))) |
| status | text | False | CHECK ((status = ANY (ARRAY['DRAFT'::text, 'PENDING_INSURANCE'::text, 'PATIENT_DUE'::text, 'PAID'::text, 'OVERDUE'::text, 'WRITTEN_OFF'::text]))) |
