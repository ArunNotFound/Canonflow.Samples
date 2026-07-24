# OKF Catalog: public.gatepasses
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| pass_id | uuid | False | None |
| resident_id | uuid | False | None |
| visitor_id | uuid | False | None |
| status | character varying | False | CHECK (((status)::text = ANY ((ARRAY['PENDING'::character varying, 'APPROVED'::character varying, 'DECLINED'::character varying, 'ENTERED'::character varying, 'EXITED'::character varying, 'EXPIRED'::character varying])::text[]))) |
| purpose | character varying | False | None |
| expected_arrival | timestamp with time zone | False | CHECK (((actual_arrival IS NULL) OR (actual_arrival >= (expected_arrival - '01:00:00'::interval)))) |
| actual_arrival | timestamp with time zone | True | CHECK (((actual_arrival IS NULL) OR (actual_arrival >= (expected_arrival - '01:00:00'::interval)))), CHECK (((actual_departure IS NULL) OR ((actual_arrival IS NOT NULL) AND (actual_departure >= actual_arrival)))) |
| actual_departure | timestamp with time zone | True | CHECK (((actual_departure IS NULL) OR ((actual_arrival IS NOT NULL) AND (actual_departure >= actual_arrival)))) |
