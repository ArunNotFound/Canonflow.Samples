CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE users (
    user_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    role VARCHAR(20) NOT NULL, -- 'CUSTOMER' or 'PROFESSIONAL'
    full_name VARCHAR(100) NOT NULL,
    phone_number VARCHAR(15) UNIQUE NOT NULL,
    email VARCHAR(100) UNIQUE,
    status VARCHAR(20) DEFAULT 'ACTIVE' NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    CHECK (role IN ('CUSTOMER', 'PROFESSIONAL')),
    CHECK (length(full_name) >= 2 AND length(full_name) <= 100),
    CHECK (length(phone_number) >= 10 AND length(phone_number) <= 15),
    CHECK (status IN ('ACTIVE', 'INACTIVE', 'BANNED'))
);

CREATE TABLE services (
    service_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    category VARCHAR(50) NOT NULL,
    name VARCHAR(100) NOT NULL,
    base_price DECIMAL(10, 2) NOT NULL,
    is_active BOOLEAN DEFAULT TRUE NOT NULL,
    
    CHECK (base_price >= 0.0),
    CHECK (category IN ('CLEANING', 'PLUMBING', 'ELECTRICAL', 'SALON', 'APPLIANCE_REPAIR'))
);

CREATE TABLE professional_services (
    professional_id UUID NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    service_id UUID NOT NULL REFERENCES services(service_id) ON DELETE CASCADE,
    experience_years INTEGER NOT NULL,
    
    PRIMARY KEY (professional_id, service_id),
    CHECK (experience_years >= 0 AND experience_years <= 50)
);

CREATE TABLE bookings (
    booking_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    customer_id UUID NOT NULL REFERENCES users(user_id),
    professional_id UUID REFERENCES users(user_id), -- Can be assigned later
    service_id UUID NOT NULL REFERENCES services(service_id),
    status VARCHAR(20) DEFAULT 'PENDING' NOT NULL,
    scheduled_time TIMESTAMP WITH TIME ZONE NOT NULL,
    total_amount DECIMAL(10, 2) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    completed_at TIMESTAMP WITH TIME ZONE,
    
    CHECK (status IN ('PENDING', 'ACCEPTED', 'IN_PROGRESS', 'COMPLETED', 'CANCELLED')),
    CHECK (total_amount >= 0.0),
    CHECK (completed_at IS NULL OR completed_at >= scheduled_time)
);
