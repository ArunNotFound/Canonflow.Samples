# OKF Catalog: public.position
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| position_id | uuid | False | None |
| account_id | uuid | False | None |
| ticker | text | False | CHECK (((length(ticker) >= 1) AND (length(ticker) <= 5))) |
| shares | integer | False | CHECK ((shares >= 0)) |
