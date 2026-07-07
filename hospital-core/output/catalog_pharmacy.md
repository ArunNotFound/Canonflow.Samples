# OKF Catalog: public.pharmacy
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| dispense_id | uuid | False | None |
| prescription_id | uuid | False | None |
| dispensed_date | timestamp with time zone | False | None |
| quantity | integer | False | CHECK ((quantity > 0)) |
| pharmacist_id | uuid | False | None |
| status | text | False | CHECK ((status = ANY (ARRAY['DISPENSED'::text, 'REJECTED_INTERACTION'::text, 'OUT_OF_STOCK'::text]))) |
