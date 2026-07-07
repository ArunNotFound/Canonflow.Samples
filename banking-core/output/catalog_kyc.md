# OKF Catalog: public.kyc
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| kyc_id | uuid | False | None |
| customer_id | uuid | False | None |
| document_type | text | False | CHECK ((document_type = ANY (ARRAY['PASSPORT'::text, 'NATIONAL_ID'::text, 'DRIVERS_LICENSE'::text]))) |
| document_number | text | False | None |
| verified | boolean | False | None |
