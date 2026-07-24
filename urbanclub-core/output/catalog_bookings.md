# OKF Catalog: public.bookings
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| booking_id | uuid | False | None |
| customer_id | uuid | False | None |
| professional_id | uuid | True | None |
| service_id | uuid | False | None |
| status | character varying | False | CHECK (((status)::text = ANY ((ARRAY['PENDING'::character varying, 'ACCEPTED'::character varying, 'IN_PROGRESS'::character varying, 'COMPLETED'::character varying, 'CANCELLED'::character varying])::text[]))) |
| scheduled_time | timestamp with time zone | False | CHECK (((completed_at IS NULL) OR (completed_at >= scheduled_time))) |
| total_amount | numeric | False | CHECK ((total_amount >= 0.0)) |
| created_at | timestamp with time zone | True | None |
| completed_at | timestamp with time zone | True | CHECK (((completed_at IS NULL) OR (completed_at >= scheduled_time))) |
