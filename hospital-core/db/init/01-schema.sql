CREATE SCHEMA IF NOT EXISTS public;

CREATE TABLE patient (
    patient_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    first_name TEXT NOT NULL,
    last_name TEXT NOT NULL,
    date_of_birth DATE NOT NULL CHECK (date_of_birth <= current_date),
    -- Age constraint: hospital policies may not allow registering deceased past a certain age, or future dates.
    gender TEXT NOT NULL CHECK (gender IN ('M', 'F', 'O')),
    blood_group TEXT CHECK (blood_group IN ('A+', 'A-', 'B+', 'B-', 'O+', 'O-', 'AB+', 'AB-')),
    status TEXT NOT NULL CHECK (status IN ('REGISTERED', 'ADMITTED', 'DISCHARGED', 'DECEASED'))
);

CREATE TABLE insurance (
    insurance_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    patient_id UUID NOT NULL REFERENCES patient(patient_id),
    provider_name TEXT NOT NULL,
    policy_number TEXT NOT NULL UNIQUE,
    coverage_limit NUMERIC(15, 2) NOT NULL CHECK (coverage_limit >= 0),
    copay_percentage INTEGER NOT NULL CHECK (copay_percentage >= 0 AND copay_percentage <= 100),
    valid_until DATE NOT NULL,
    status TEXT NOT NULL CHECK (status IN ('ACTIVE', 'EXPIRED', 'SUSPENDED'))
);

CREATE TABLE doctor (
    doctor_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    license_number TEXT NOT NULL UNIQUE,
    full_name TEXT NOT NULL,
    specialty TEXT NOT NULL CHECK (specialty IN ('GENERAL', 'CARDIOLOGY', 'NEUROLOGY', 'PEDIATRICS', 'ONCOLOGY', 'SURGERY')),
    status TEXT NOT NULL CHECK (status IN ('ACTIVE', 'ON_LEAVE', 'RETIRED', 'TERMINATED'))
);

CREATE TABLE visit (
    visit_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    patient_id UUID NOT NULL REFERENCES patient(patient_id),
    doctor_id UUID NOT NULL REFERENCES doctor(doctor_id),
    visit_time TIMESTAMPTZ NOT NULL DEFAULT now(),
    type TEXT NOT NULL CHECK (type IN ('OUTPATIENT', 'INPATIENT', 'EMERGENCY')),
    status TEXT NOT NULL CHECK (status IN ('SCHEDULED', 'IN_PROGRESS', 'COMPLETED', 'CANCELLED'))
);

CREATE TABLE prescription (
    prescription_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    visit_id UUID NOT NULL REFERENCES visit(visit_id),
    doctor_id UUID NOT NULL REFERENCES doctor(doctor_id),
    medication_name TEXT NOT NULL,
    dosage TEXT NOT NULL,
    frequency TEXT NOT NULL,
    duration_days INTEGER NOT NULL CHECK (duration_days > 0 AND duration_days <= 365),
    -- Drug restriction: controlled substances require explicit flag
    is_controlled_substance BOOLEAN NOT NULL DEFAULT false,
    status TEXT NOT NULL CHECK (status IN ('ACTIVE', 'FILLED', 'CANCELLED', 'EXPIRED'))
);

CREATE TABLE pharmacy (
    dispense_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    prescription_id UUID NOT NULL REFERENCES prescription(prescription_id),
    dispensed_date TIMESTAMPTZ NOT NULL DEFAULT now(),
    quantity INTEGER NOT NULL CHECK (quantity > 0),
    pharmacist_id UUID NOT NULL,
    status TEXT NOT NULL CHECK (status IN ('DISPENSED', 'REJECTED_INTERACTION', 'OUT_OF_STOCK'))
);

CREATE TABLE lab (
    lab_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    visit_id UUID NOT NULL REFERENCES visit(visit_id),
    test_name TEXT NOT NULL,
    result_value TEXT,
    is_abnormal BOOLEAN,
    ordered_time TIMESTAMPTZ NOT NULL DEFAULT now(),
    completed_time TIMESTAMPTZ,
    status TEXT NOT NULL CHECK (status IN ('ORDERED', 'IN_PROGRESS', 'COMPLETED', 'CANCELLED')),
    CONSTRAINT lab_timeline CHECK (completed_time IS NULL OR completed_time >= ordered_time)
);

CREATE TABLE billing (
    bill_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    visit_id UUID NOT NULL REFERENCES visit(visit_id),
    total_amount NUMERIC(15, 2) NOT NULL CHECK (total_amount >= 0),
    patient_responsibility NUMERIC(15, 2) NOT NULL CHECK (patient_responsibility >= 0),
    insurance_responsibility NUMERIC(15, 2) NOT NULL CHECK (insurance_responsibility >= 0),
    status TEXT NOT NULL CHECK (status IN ('DRAFT', 'PENDING_INSURANCE', 'PATIENT_DUE', 'PAID', 'OVERDUE', 'WRITTEN_OFF')),
    CONSTRAINT billing_math CHECK (total_amount = patient_responsibility + insurance_responsibility)
);

CREATE TABLE claims (
    claim_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    bill_id UUID NOT NULL REFERENCES billing(bill_id),
    insurance_id UUID NOT NULL REFERENCES insurance(insurance_id),
    claimed_amount NUMERIC(15, 2) NOT NULL CHECK (claimed_amount > 0),
    approved_amount NUMERIC(15, 2) CHECK (approved_amount >= 0 AND approved_amount <= claimed_amount),
    status TEXT NOT NULL CHECK (status IN ('SUBMITTED', 'UNDER_REVIEW', 'APPROVED', 'PARTIAL_APPROVED', 'REJECTED')),
    submitted_date TIMESTAMPTZ NOT NULL DEFAULT now()
);
