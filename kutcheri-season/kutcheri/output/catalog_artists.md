# OKF Catalog: public.artists
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| artist_id | uuid | False | None |
| name | text | False | None |
| form | text | False | CHECK ((form = ANY (ARRAY['Vocal'::text, 'Violin'::text, 'Veena'::text, 'Mridangam'::text, 'Flute'::text, 'Ensemble'::text]))) |
| contact | text | True | None |
| standard_fee_band | numeric | False | CHECK ((standard_fee_band >= (0)::numeric)) |
