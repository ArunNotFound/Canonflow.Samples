CREATE SCHEMA IF NOT EXISTS public;

CREATE TABLE passenger (
    passenger_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    passport_number TEXT NOT NULL UNIQUE,
    full_name TEXT NOT NULL,
    dob DATE NOT NULL CHECK (dob < current_date),
    status TEXT NOT NULL CHECK (status IN ('ACTIVE', 'BANNED'))
);

CREATE TABLE loyalty (
    loyalty_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    passenger_id UUID NOT NULL REFERENCES passenger(passenger_id),
    tier TEXT NOT NULL CHECK (tier IN ('BLUE', 'SILVER', 'GOLD', 'PLATINUM')),
    points INTEGER NOT NULL DEFAULT 0 CHECK (points >= 0)
);

CREATE TABLE aircraft (
    aircraft_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tail_number TEXT NOT NULL UNIQUE,
    model TEXT NOT NULL,
    max_capacity INTEGER NOT NULL CHECK (max_capacity > 0)
);

CREATE TABLE seat (
    aircraft_id UUID NOT NULL REFERENCES aircraft(aircraft_id),
    seat_number TEXT NOT NULL,
    class TEXT NOT NULL CHECK (class IN ('ECONOMY', 'PREMIUM', 'BUSINESS', 'FIRST')),
    PRIMARY KEY (aircraft_id, seat_number)
);

CREATE TABLE flight (
    flight_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    flight_number TEXT NOT NULL,
    aircraft_id UUID NOT NULL REFERENCES aircraft(aircraft_id),
    departure_time TIMESTAMPTZ NOT NULL,
    arrival_time TIMESTAMPTZ NOT NULL,
    origin TEXT NOT NULL CHECK (length(origin) = 3),
    destination TEXT NOT NULL CHECK (length(destination) = 3),
    status TEXT NOT NULL CHECK (status IN ('SCHEDULED', 'BOARDING', 'DEPARTED', 'ARRIVED', 'CANCELLED')),
    CONSTRAINT flight_time_check CHECK (arrival_time > departure_time),
    CONSTRAINT flight_route_check CHECK (origin <> destination),
    UNIQUE (flight_id, aircraft_id) -- Needed for overlapping foreign keys in booking
);

CREATE TABLE crew (
    crew_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    employee_number TEXT NOT NULL UNIQUE,
    role TEXT NOT NULL CHECK (role IN ('PILOT', 'COPILOT', 'FLIGHT_ATTENDANT', 'PURSER')),
    status TEXT NOT NULL CHECK (status IN ('ACTIVE', 'REST', 'ON_LEAVE'))
);

CREATE TABLE flight_crew (
    flight_id UUID NOT NULL REFERENCES flight(flight_id),
    crew_id UUID NOT NULL REFERENCES crew(crew_id),
    PRIMARY KEY (flight_id, crew_id)
);

CREATE TABLE booking (
    booking_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    passenger_id UUID NOT NULL REFERENCES passenger(passenger_id),
    flight_id UUID NOT NULL,
    aircraft_id UUID NOT NULL,
    seat_number TEXT NOT NULL,
    booking_time TIMESTAMPTZ NOT NULL DEFAULT now(),
    status TEXT NOT NULL CHECK (status IN ('HOLD', 'CONFIRMED', 'CANCELLED')),
    
    -- INTERCONNECTED RULE 1: The booking must reference a valid flight and its assigned aircraft.
    FOREIGN KEY (flight_id, aircraft_id) REFERENCES flight(flight_id, aircraft_id),
    
    -- INTERCONNECTED RULE 2: The booked seat must actually exist on that specific aircraft.
    FOREIGN KEY (aircraft_id, seat_number) REFERENCES seat(aircraft_id, seat_number)
);

CREATE TABLE payment (
    payment_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    booking_id UUID NOT NULL REFERENCES booking(booking_id),
    amount NUMERIC(15, 2) NOT NULL CHECK (amount > 0),
    currency TEXT NOT NULL CHECK (length(currency) = 3),
    status TEXT NOT NULL CHECK (status IN ('PENDING', 'AUTHORIZED', 'CAPTURED', 'REFUNDED', 'FAILED')),
    timestamp TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE ticket (
    ticket_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    ticket_number TEXT NOT NULL UNIQUE CHECK (length(ticket_number) = 13),
    booking_id UUID NOT NULL REFERENCES booking(booking_id),
    issue_date TIMESTAMPTZ NOT NULL DEFAULT now(),
    status TEXT NOT NULL CHECK (status IN ('ISSUED', 'USED', 'VOID'))
);
