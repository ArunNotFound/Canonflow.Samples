# OKF Catalog: public.ticket
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| ticket_id | uuid | False | None |
| ticket_number | text | False | CHECK ((length(ticket_number) = 13)) |
| booking_id | uuid | False | None |
| issue_date | timestamp with time zone | False | None |
| status | text | False | CHECK ((status = ANY (ARRAY['ISSUED'::text, 'USED'::text, 'VOID'::text]))) |
