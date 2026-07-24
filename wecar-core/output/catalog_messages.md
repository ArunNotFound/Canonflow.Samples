# OKF Catalog: public.messages
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| message_id | uuid | False | None |
| sender_id | uuid | False | None |
| receiver_id | uuid | True | CHECK ((((receiver_id IS NOT NULL) AND (group_id IS NULL)) OR ((receiver_id IS NULL) AND (group_id IS NOT NULL)))) |
| group_id | uuid | True | CHECK ((((receiver_id IS NOT NULL) AND (group_id IS NULL)) OR ((receiver_id IS NULL) AND (group_id IS NOT NULL)))) |
| content | text | False | CHECK (((length(content) > 0) AND (length(content) <= 4000))) |
| message_type | character varying | False | CHECK (((message_type)::text = ANY ((ARRAY['TEXT'::character varying, 'IMAGE'::character varying, 'VIDEO'::character varying, 'AUDIO'::character varying, 'FILE'::character varying])::text[]))) |
| sent_at | timestamp with time zone | True | CHECK (((read_at IS NULL) OR (read_at >= sent_at))) |
| read_at | timestamp with time zone | True | CHECK (((read_at IS NULL) OR (read_at >= sent_at))) |
