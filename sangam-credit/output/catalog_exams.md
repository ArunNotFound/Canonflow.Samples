# OKF Catalog: public.exams
**Overall Lineage Grade:** Declared

| Column | Type | Nullable | Checks |
|---|---|---|---|
| exam_id | integer | False | None |
| student_id | integer | False | None |
| batch_id | integer | False | None |
| marks | integer | False | CHECK (((marks >= 0) AND (marks <= 100))) |
| theory_marks | integer | False | CHECK (((theory_marks + practical_marks) <= 100)) |
| practical_marks | integer | False | CHECK (((theory_marks + practical_marks) <= 100)) |
