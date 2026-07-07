# OKF Catalog: public.kutcheris
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| kutcheri_id | uuid | False | None |
| artist_id | uuid | False | None |
| venue_id | uuid | False | None |
| start_time | timestamp with time zone | False | CHECK ((end_time > start_time)) |
| end_time | timestamp with time zone | False | CHECK ((end_time > start_time)) |
| status | text | False | CHECK ((status = ANY (ARRAY['draft'::text, 'on-sale'::text, 'confirmed'::text, 'sold-out'::text, 'on-hold'::text, 'completed'::text, 'cancelled'::text]))) |
| artist_fee | numeric | False | CHECK ((artist_fee >= (0)::numeric)) |
| venue_cost | numeric | False | CHECK ((venue_cost >= (0)::numeric)) |
