# OKF Catalog: public.aircraft
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| aircraft_id | uuid | False | None |
| tail_number | text | False | None |
| model | text | False | None |
| max_capacity | integer | False | CHECK ((max_capacity > 0)) |
