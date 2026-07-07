# OKF Catalog: public.bookings
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| booking_id | uuid | False | None |
| tier_id | uuid | False | None |
| quantity | integer | False | CHECK ((quantity > 0)) |
| channel | text | False | None |
| timestamp | timestamp with time zone | False | None |
