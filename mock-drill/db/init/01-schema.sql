CREATE SCHEMA IF NOT EXISTS public;

CREATE TABLE customers (
  customer_id UUID PRIMARY KEY,
  email VARCHAR(255) NOT NULL UNIQUE,
  age INT NOT NULL CHECK (age >= 18 AND age < 120),
  status VARCHAR(20) NOT NULL CHECK (status IN ('ACTIVE', 'SUSPENDED', 'CLOSED')),
  credit_limit DECIMAL(12,2) NOT NULL CHECK (credit_limit >= 0 AND credit_limit <= 1000000),
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE orders (
  order_id UUID PRIMARY KEY,
  customer_id UUID NOT NULL REFERENCES customers(customer_id),
  amount DECIMAL(12,2) NOT NULL CHECK (amount > 0),
  currency CHAR(3) NOT NULL CHECK (currency IN ('INR', 'USD', 'EUR')),
  order_status VARCHAR(20) NOT NULL CHECK (order_status IN ('PLACED', 'PAID', 'CANCELLED')),
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);
