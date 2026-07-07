# OKF Catalog: public.crew
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| crew_id | uuid | False | None |
| employee_number | text | False | None |
| role | text | False | CHECK ((role = ANY (ARRAY['PILOT'::text, 'COPILOT'::text, 'FLIGHT_ATTENDANT'::text, 'PURSER'::text]))) |
| status | text | False | CHECK ((status = ANY (ARRAY['ACTIVE'::text, 'REST'::text, 'ON_LEAVE'::text]))) |
