# OKF Catalog: public.ticket_tiers
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| tier_id | uuid | False | None |
| kutcheri_id | uuid | False | None |
| name | text | False | None |
| price | numeric | False | CHECK ((price >= (0)::numeric)) |
| allocation | integer | False | CHECK ((allocation >= 0)), CHECK ((sold_count <= allocation)) |
| sold_count | integer | False | CHECK ((sold_count <= allocation)), CHECK ((sold_count >= 0)) |
