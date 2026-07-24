# OKF Catalog: public.visitors
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| visitor_id | uuid | False | None |
| full_name | character varying | False | None |
| phone | character varying | False | CHECK (((length((phone)::text) >= 10) AND (length((phone)::text) <= 15))) |
| id_proof_type | character varying | False | CHECK (((id_proof_type)::text = ANY ((ARRAY['AADHAAR'::character varying, 'PAN'::character varying, 'PASSPORT'::character varying, 'DRIVING_LICENSE'::character varying, 'OTHER'::character varying])::text[]))) |
