CREATE SCHEMA IF NOT EXISTS public;

CREATE TABLE users (
    user_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    email TEXT NOT NULL UNIQUE,
    status TEXT NOT NULL DEFAULT 'ACTIVE' CHECK (status IN ('ACTIVE', 'BANNED')), -- NEW COLUMN
    created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE products ( -- NEW TABLE
    product_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name TEXT NOT NULL,
    price NUMERIC(10, 2) NOT NULL CHECK (price >= 0)
);

CREATE TABLE orders (
    order_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(user_id),
    total_amount NUMERIC(10, 2) NOT NULL CHECK (total_amount >= 0), -- RELAXED CONSTRAINT
    discount NUMERIC(10, 2) NOT NULL DEFAULT 0 CHECK (discount >= 0) -- NEW COLUMN
);
