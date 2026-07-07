# OKF Catalog: public.notifications
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| notification_id | uuid | False | None |
| customer_id | uuid | False | None |
| channel | text | False | CHECK ((channel = ANY (ARRAY['SMS'::text, 'EMAIL'::text, 'PUSH'::text]))) |
| message | text | False | None |
| status | text | False | CHECK ((status = ANY (ARRAY['PENDING'::text, 'SENT'::text, 'FAILED'::text]))) |
| created_at | timestamp with time zone | False | None |
