-- The Silent Schema: rules live in the app, not the DB. Harvest comes back
-- nearly empty. The product must SAY SO, loudly and honestly.
CREATE TABLE customers (
  id SERIAL PRIMARY KEY, email VARCHAR(255), phone VARCHAR(255),
  age VARCHAR(255), country VARCHAR(255), status VARCHAR(255), tier VARCHAR(255)
);  -- every rule (email format, age>=18, status enum) lives in Rails validators
CREATE TABLE orders (
  id SERIAL PRIMARY KEY, customer_id INT, total VARCHAR(255),
  currency VARCHAR(255), state VARCHAR(255), created VARCHAR(255)
);  -- total>0, currency enum, state machine — all in app code, none in DB
