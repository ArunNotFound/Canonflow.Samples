CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE residents (
    resident_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    full_name VARCHAR(100) NOT NULL,
    unit_number VARCHAR(20) NOT NULL,
    phone VARCHAR(15) NOT NULL,
    email VARCHAR(100),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    
    CONSTRAINT chk_resident_phone CHECK (length(phone) >= 10 AND length(phone) <= 15),
    CONSTRAINT chk_resident_unit CHECK (length(unit_number) > 0)
);

CREATE TABLE visitors (
    visitor_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    full_name VARCHAR(100) NOT NULL,
    phone VARCHAR(15) NOT NULL,
    id_proof_type VARCHAR(20) NOT NULL,
    
    CONSTRAINT chk_visitor_phone CHECK (length(phone) >= 10 AND length(phone) <= 15),
    CONSTRAINT chk_visitor_id_proof CHECK (id_proof_type IN ('AADHAAR', 'PAN', 'PASSPORT', 'DRIVING_LICENSE', 'OTHER'))
);

CREATE TABLE gatepasses (
    pass_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    resident_id UUID NOT NULL REFERENCES residents(resident_id),
    visitor_id UUID NOT NULL REFERENCES visitors(visitor_id),
    status VARCHAR(20) NOT NULL DEFAULT 'PENDING',
    purpose VARCHAR(100) NOT NULL,
    expected_arrival TIMESTAMP WITH TIME ZONE NOT NULL,
    actual_arrival TIMESTAMP WITH TIME ZONE,
    actual_departure TIMESTAMP WITH TIME ZONE,
    
    CONSTRAINT chk_pass_status CHECK (status IN ('PENDING', 'APPROVED', 'DECLINED', 'ENTERED', 'EXITED', 'EXPIRED')),
    CONSTRAINT chk_arrival_timeline CHECK (actual_arrival IS NULL OR actual_arrival >= expected_arrival - interval '1 hour'),
    CONSTRAINT chk_departure_timeline CHECK (actual_departure IS NULL OR (actual_arrival IS NOT NULL AND actual_departure >= actual_arrival))
);
