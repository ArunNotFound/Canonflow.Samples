CREATE SCHEMA IF NOT EXISTS public;

CREATE TABLE branch (
    branch_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    branch_code TEXT NOT NULL UNIQUE CHECK (length(branch_code) = 4),
    name TEXT NOT NULL,
    status TEXT NOT NULL CHECK (status IN ('ACTIVE', 'CLOSED', 'SUSPENDED'))
);

CREATE TABLE employee (
    employee_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    branch_id UUID NOT NULL REFERENCES branch(branch_id),
    name TEXT NOT NULL,
    role TEXT NOT NULL CHECK (role IN ('TELLER', 'MANAGER', 'AUDITOR', 'OFFICER'))
);

CREATE TABLE customer (
    customer_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    cif_number TEXT NOT NULL UNIQUE CHECK (length(cif_number) = 8),
    full_name TEXT NOT NULL,
    date_of_birth DATE NOT NULL CHECK (date_of_birth < current_date),
    risk_rating TEXT NOT NULL CHECK (risk_rating IN ('LOW', 'MEDIUM', 'HIGH')),
    status TEXT NOT NULL CHECK (status IN ('ACTIVE', 'INACTIVE', 'DORMANT'))
);

CREATE TABLE kyc (
    kyc_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    customer_id UUID NOT NULL REFERENCES customer(customer_id),
    document_type TEXT NOT NULL CHECK (document_type IN ('PASSPORT', 'NATIONAL_ID', 'DRIVERS_LICENSE')),
    document_number TEXT NOT NULL,
    verified BOOLEAN NOT NULL DEFAULT false
);

CREATE TABLE aml (
    aml_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    customer_id UUID NOT NULL REFERENCES customer(customer_id),
    screening_status TEXT NOT NULL CHECK (screening_status IN ('CLEARED', 'FLAGGED', 'UNDER_REVIEW')),
    last_screened TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE currency (
    currency_code TEXT PRIMARY KEY CHECK (length(currency_code) = 3),
    name TEXT NOT NULL
);

CREATE TABLE exchange_rate (
    rate_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    base_currency TEXT NOT NULL REFERENCES currency(currency_code),
    target_currency TEXT NOT NULL REFERENCES currency(currency_code),
    rate NUMERIC(10, 6) NOT NULL CHECK (rate > 0),
    effective_date TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE account (
    account_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    customer_id UUID NOT NULL REFERENCES customer(customer_id),
    branch_id UUID NOT NULL REFERENCES branch(branch_id),
    currency TEXT NOT NULL REFERENCES currency(currency_code),
    account_type TEXT NOT NULL CHECK (account_type IN ('SAVINGS', 'CURRENT', 'LOAN', 'FD')),
    balance NUMERIC(15, 2) NOT NULL DEFAULT 0.00,
    status TEXT NOT NULL CHECK (status IN ('OPEN', 'CLOSED', 'FROZEN'))
);

CREATE TABLE limits (
    limit_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    account_id UUID NOT NULL REFERENCES account(account_id),
    daily_transfer_limit NUMERIC(15, 2) NOT NULL CHECK (daily_transfer_limit >= 0),
    overdraft_limit NUMERIC(15, 2) NOT NULL CHECK (overdraft_limit >= 0)
);

CREATE TABLE nominee (
    nominee_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    account_id UUID NOT NULL REFERENCES account(account_id),
    full_name TEXT NOT NULL,
    relationship TEXT NOT NULL,
    allocation_percentage INTEGER NOT NULL CHECK (allocation_percentage > 0 AND allocation_percentage <= 100)
);

CREATE TABLE interest_rate (
    rate_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    account_type TEXT NOT NULL,
    rate_percentage NUMERIC(5, 2) NOT NULL CHECK (rate_percentage >= 0 AND rate_percentage <= 100),
    effective_date TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE fixed_deposit (
    fd_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    account_id UUID NOT NULL REFERENCES account(account_id),
    principal NUMERIC(15, 2) NOT NULL CHECK (principal > 0),
    maturity_date DATE NOT NULL,
    status TEXT NOT NULL CHECK (status IN ('ACTIVE', 'MATURED', 'BROKEN'))
);

CREATE TABLE loan (
    loan_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    account_id UUID NOT NULL REFERENCES account(account_id),
    principal_amount NUMERIC(15, 2) NOT NULL CHECK (principal_amount > 0),
    outstanding_balance NUMERIC(15, 2) NOT NULL CHECK (outstanding_balance >= 0),
    interest_rate NUMERIC(5, 2) NOT NULL CHECK (interest_rate >= 0),
    status TEXT NOT NULL CHECK (status IN ('DISBURSED', 'CLOSED', 'DEFAULTED'))
);

CREATE TABLE card (
    card_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    account_id UUID NOT NULL REFERENCES account(account_id),
    card_number TEXT NOT NULL UNIQUE CHECK (length(card_number) >= 16),
    card_type TEXT NOT NULL CHECK (card_type IN ('DEBIT', 'CREDIT', 'PREPAID')),
    status TEXT NOT NULL CHECK (status IN ('ACTIVE', 'BLOCKED', 'EXPIRED'))
);

CREATE TABLE transaction (
    txn_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    account_id UUID NOT NULL REFERENCES account(account_id),
    amount NUMERIC(15, 2) NOT NULL CHECK (amount <> 0),
    txn_type TEXT NOT NULL CHECK (txn_type IN ('CREDIT', 'DEBIT')),
    currency TEXT NOT NULL REFERENCES currency(currency_code),
    timestamp TIMESTAMPTZ NOT NULL DEFAULT now(),
    reference TEXT NOT NULL
);

CREATE TABLE ledger (
    ledger_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    txn_id UUID NOT NULL REFERENCES transaction(txn_id),
    gl_account TEXT NOT NULL,
    entry_type TEXT NOT NULL CHECK (entry_type IN ('DR', 'CR')),
    amount NUMERIC(15, 2) NOT NULL CHECK (amount > 0)
);

CREATE TABLE charges (
    charge_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    txn_id UUID NOT NULL REFERENCES transaction(txn_id),
    charge_type TEXT NOT NULL CHECK (charge_type IN ('FEE', 'TAX', 'PENALTY')),
    amount NUMERIC(15, 2) NOT NULL CHECK (amount > 0)
);

CREATE TABLE statement (
    statement_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    account_id UUID NOT NULL REFERENCES account(account_id),
    period_start DATE NOT NULL,
    period_end DATE NOT NULL,
    opening_balance NUMERIC(15, 2) NOT NULL,
    closing_balance NUMERIC(15, 2) NOT NULL,
    CONSTRAINT valid_period CHECK (period_end > period_start)
);

CREATE TABLE audit (
    audit_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    table_name TEXT NOT NULL,
    record_id UUID NOT NULL,
    action TEXT NOT NULL CHECK (action IN ('INSERT', 'UPDATE', 'DELETE')),
    timestamp TIMESTAMPTZ NOT NULL DEFAULT now(),
    actor_id UUID
);

CREATE TABLE notifications (
    notification_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    customer_id UUID NOT NULL REFERENCES customer(customer_id),
    channel TEXT NOT NULL CHECK (channel IN ('SMS', 'EMAIL', 'PUSH')),
    message TEXT NOT NULL,
    status TEXT NOT NULL CHECK (status IN ('PENDING', 'SENT', 'FAILED')),
    created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);
