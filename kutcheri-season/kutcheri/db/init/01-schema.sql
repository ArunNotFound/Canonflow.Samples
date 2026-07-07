CREATE SCHEMA IF NOT EXISTS public;

CREATE TABLE artists (
    artist_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name TEXT NOT NULL,
    form TEXT NOT NULL CHECK (form IN ('Vocal', 'Violin', 'Veena', 'Mridangam', 'Flute', 'Ensemble')),
    contact TEXT,
    standard_fee_band NUMERIC(10, 2) NOT NULL CHECK (standard_fee_band >= 0)
);

CREATE TABLE venues (
    venue_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name TEXT NOT NULL,
    seating_capacity INTEGER NOT NULL CHECK (seating_capacity > 0),
    address TEXT NOT NULL
);

CREATE TABLE kutcheris (
    kutcheri_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    artist_id UUID NOT NULL REFERENCES artists(artist_id),
    venue_id UUID NOT NULL REFERENCES venues(venue_id),
    start_time TIMESTAMPTZ NOT NULL,
    end_time TIMESTAMPTZ NOT NULL,
    status TEXT NOT NULL CHECK (status IN ('draft', 'on-sale', 'confirmed', 'sold-out', 'on-hold', 'completed', 'cancelled')),
    artist_fee NUMERIC(10, 2) NOT NULL CHECK (artist_fee >= 0),
    venue_cost NUMERIC(10, 2) NOT NULL CHECK (venue_cost >= 0),
    CONSTRAINT kutcheri_time_check CHECK (end_time > start_time)
);

-- Note: Overlapping windows (FR-9, FR-10) typically require EXCLUDE using gist(tsrange) 
-- EXCLUDE USING gist (venue_id WITH =, tsrange(start_time, end_time) WITH &&)

CREATE TABLE ticket_tiers (
    tier_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    kutcheri_id UUID NOT NULL REFERENCES kutcheris(kutcheri_id),
    name TEXT NOT NULL,
    price NUMERIC(10, 2) NOT NULL CHECK (price >= 0),
    allocation INTEGER NOT NULL CHECK (allocation >= 0),
    sold_count INTEGER NOT NULL DEFAULT 0 CHECK (sold_count >= 0),
    CONSTRAINT tier_allocation_check CHECK (sold_count <= allocation)
);

CREATE TABLE bookings (
    booking_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tier_id UUID NOT NULL REFERENCES ticket_tiers(tier_id),
    quantity INTEGER NOT NULL CHECK (quantity > 0),
    channel TEXT NOT NULL,
    timestamp TIMESTAMPTZ NOT NULL DEFAULT now()
);
