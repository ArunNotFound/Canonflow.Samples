CREATE SCHEMA IF NOT EXISTS public;

CREATE TABLE account (
    account_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    owner_name TEXT NOT NULL,
    cash_balance NUMERIC(15, 2) NOT NULL CHECK (cash_balance >= 0) -- DB CONSTRAINT: Cannot withdraw below 0
);

CREATE TABLE position (
    position_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    account_id UUID NOT NULL REFERENCES account(account_id),
    ticker TEXT NOT NULL CHECK (length(ticker) BETWEEN 1 AND 5), -- DB CONSTRAINT: Valid Ticker length
    shares INTEGER NOT NULL CHECK (shares >= 0), -- DB CONSTRAINT: No naked shorts allowed physically
    UNIQUE (account_id, ticker)
);

CREATE TABLE trade_order (
    order_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    account_id UUID NOT NULL REFERENCES account(account_id),
    ticker TEXT NOT NULL CHECK (length(ticker) BETWEEN 1 AND 5),
    side TEXT NOT NULL CHECK (side IN ('BUY', 'SELL')),
    quantity INTEGER NOT NULL CHECK (quantity > 0),
    limit_price NUMERIC(15, 2) NOT NULL CHECK (limit_price > 0),
    status TEXT NOT NULL CHECK (status IN ('PENDING', 'FILLED', 'REJECTED')),
    submitted_at TIMESTAMPTZ NOT NULL DEFAULT now()
);
