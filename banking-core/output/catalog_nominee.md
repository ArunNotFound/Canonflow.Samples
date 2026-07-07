# OKF Catalog: public.nominee
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| nominee_id | uuid | False | None |
| account_id | uuid | False | None |
| full_name | text | False | None |
| relationship | text | False | None |
| allocation_percentage | integer | False | CHECK (((allocation_percentage > 0) AND (allocation_percentage <= 100))) |
