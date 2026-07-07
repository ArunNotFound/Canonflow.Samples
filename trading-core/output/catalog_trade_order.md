# OKF Catalog: public.trade_order
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| order_id | uuid | False | None |
| account_id | uuid | False | None |
| ticker | text | False | CHECK (((length(ticker) >= 1) AND (length(ticker) <= 5))) |
| side | text | False | CHECK ((side = ANY (ARRAY['BUY'::text, 'SELL'::text]))) |
| quantity | integer | False | CHECK ((quantity > 0)) |
| limit_price | numeric | False | CHECK ((limit_price > (0)::numeric)) |
| status | text | False | CHECK ((status = ANY (ARRAY['PENDING'::text, 'FILLED'::text, 'REJECTED'::text]))) |
| submitted_at | timestamp with time zone | False | None |
