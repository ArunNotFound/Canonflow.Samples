# OKF Catalog: public.gurus
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| guru_id | integer | False | None |
| full_name | character varying | False | None |
| email | character varying | False | None |
| years_experience | integer | False | CHECK ((years_experience > 0)) |
| specialization | character varying | False | CHECK (((specialization)::text = ANY ((ARRAY['vocal'::character varying, 'violin'::character varying, 'veena'::character varying, 'mridangam'::character varying, 'flute'::character varying])::text[]))) |
