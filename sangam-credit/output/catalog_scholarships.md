# OKF Catalog: public.scholarships
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| scholarship_id | integer | False | None |
| student_id | integer | False | None |
| pct_waiver | numeric | False | CHECK (((pct_waiver > (0)::numeric) AND (pct_waiver <= (100)::numeric))) |
| min_attendance_pct | numeric | False | CHECK ((min_attendance_pct > (90)::numeric)), CHECK ((min_attendance_pct < (75)::numeric)) |
