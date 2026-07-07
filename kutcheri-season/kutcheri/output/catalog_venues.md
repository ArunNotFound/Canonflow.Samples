# OKF Catalog: public.venues
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| venue_id | uuid | False | None |
| name | text | False | None |
| seating_capacity | integer | False | CHECK ((seating_capacity > 0)) |
| address | text | False | None |
