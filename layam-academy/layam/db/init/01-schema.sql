-- Layam Academy — dogfood schema. Every constraint is a test (see README).
CREATE TABLE gurus (
  guru_id        SERIAL PRIMARY KEY,
  full_name      VARCHAR(120) NOT NULL,
  email          VARCHAR(254) NOT NULL UNIQUE,
  years_experience INT NOT NULL CHECK (years_experience > 0),
  -- IN-list: expected to surface as Opaque (classified loss), not vanish
  specialization VARCHAR(20) NOT NULL
    CONSTRAINT guru_specialization_known
    CHECK (specialization IN ('vocal','violin','veena','mridangam','flute'))
);

CREATE TABLE students (
  student_id   SERIAL PRIMARY KEY,
  full_name    VARCHAR(120) NOT NULL,
  email        VARCHAR(254) NOT NULL UNIQUE,
  phone        VARCHAR(10)  NOT NULL CHECK (length(phone) = 10),  -- function call -> Opaque
  age          INT NOT NULL CONSTRAINT student_age_window CHECK (age >= 5 AND age <= 90),
  enrolled_on  DATE NOT NULL DEFAULT CURRENT_DATE
);

CREATE TABLE batches (
  batch_id     SERIAL PRIMARY KEY,
  guru_id      INT NOT NULL REFERENCES gurus(guru_id),
  raga_focus   VARCHAR(60),
  level        INT NOT NULL CONSTRAINT batch_level_window CHECK (level >= 1 AND level <= 8),
  capacity     INT NOT NULL CONSTRAINT batch_capacity_window CHECK (capacity > 0 AND capacity <= 12),
  fee_monthly  NUMERIC(8,2) NOT NULL
    CONSTRAINT batch_fee_window CHECK (fee_monthly >= 500 AND fee_monthly <= 15000)
);

CREATE TABLE enrollments (
  student_id   INT NOT NULL REFERENCES students(student_id),
  batch_id     INT NOT NULL REFERENCES batches(batch_id),
  discount_pct NUMERIC(4,1) NOT NULL DEFAULT 0
    CONSTRAINT enrollment_discount_window CHECK (discount_pct >= 0 AND discount_pct <= 25),
  PRIMARY KEY (student_id, batch_id)                    -- composite key (T7)
);

CREATE TABLE exams (
  exam_id         SERIAL PRIMARY KEY,
  student_id      INT NOT NULL REFERENCES students(student_id),
  batch_id        INT NOT NULL REFERENCES batches(batch_id),
  marks           INT NOT NULL CONSTRAINT exam_marks_window CHECK (marks >= 0 AND marks <= 100),
  theory_marks    INT NOT NULL DEFAULT 0,
  practical_marks INT NOT NULL DEFAULT 0,
  -- arithmetic across columns -> expected Opaque (classified loss)
  CONSTRAINT exam_split_total CHECK (theory_marks + practical_marks <= 100)
);

CREATE TABLE payments (
  payment_id  SERIAL PRIMARY KEY,
  student_id  INT NOT NULL REFERENCES students(student_id),
  batch_id    INT NOT NULL REFERENCES batches(batch_id),
  amount      NUMERIC(8,2) NOT NULL CHECK (amount > 0),
  method      VARCHAR(10) NOT NULL
    CONSTRAINT payment_method_known CHECK (method IN ('upi','card','cash')),
  paid_on     TIMESTAMPTZ NOT NULL DEFAULT now(),
  FOREIGN KEY (student_id, batch_id) REFERENCES enrollments(student_id, batch_id)  -- composite FK
);

-- SPECIMEN 1 (deliberate): contradictory constraint pair. CanonFlow's
-- semantic optimizer (ADR-015) must collapse the conjunction to False and
-- emit a diagnostic naming both constraints. DO NOT "FIX" THIS TABLE.
CREATE TABLE scholarships (
  scholarship_id     SERIAL PRIMARY KEY,
  student_id         INT NOT NULL REFERENCES students(student_id),
  pct_waiver         NUMERIC(4,1) NOT NULL CHECK (pct_waiver > 0 AND pct_waiver <= 100),
  min_attendance_pct NUMERIC(4,1) NOT NULL,
  CONSTRAINT scholarship_attendance_floor   CHECK (min_attendance_pct > 90),
  CONSTRAINT scholarship_attendance_ceiling CHECK (min_attendance_pct < 75)
);
