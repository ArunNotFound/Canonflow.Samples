-- Database Source of Truth for Fintech Wallet

CREATE TABLE public.wallets (
    wallet_id uuid PRIMARY KEY,
    customer_id uuid NOT NULL,
    currency character varying(3) NOT NULL,
    status character varying(20) NOT NULL 
        CHECK (status IN ('ACTIVE', 'SUSPENDED', 'CLOSED')),
    created_at timestamp with time zone NOT NULL
);

CREATE TABLE public.ledger_transactions (
    transaction_id uuid PRIMARY KEY,
    wallet_id uuid NOT NULL REFERENCES public.wallets(wallet_id),
    amount numeric(19, 4) NOT NULL 
        CHECK (amount > 0), -- Amount must always be positive (direction handles credit/debit)
    currency character varying(3) NOT NULL,
    direction character varying(6) NOT NULL 
        CHECK (direction IN ('CREDIT', 'DEBIT')),
    status character varying(20) NOT NULL 
        CHECK (status IN ('CREATED', 'PENDING', 'COMPLETED', 'FAILED', 'REVERSED')),
    reference_id character varying(100) NOT NULL,
    created_at timestamp with time zone NOT NULL,
    
    -- Idempotency constraint: cannot have two successful transactions with the same reference_id for a given wallet
    CONSTRAINT uq_wallet_reference UNIQUE (wallet_id, reference_id)
);

CREATE TABLE public.wallet_balances (
    wallet_id uuid PRIMARY KEY REFERENCES public.wallets(wallet_id),
    available_balance numeric(19, 4) NOT NULL 
        CHECK (available_balance >= 0), -- Wallet cannot go negative
    locked_balance numeric(19, 4) NOT NULL 
        CHECK (locked_balance >= 0),
    updated_at timestamp with time zone NOT NULL
);
