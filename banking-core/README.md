# Banking Core System

A massive demonstration of a Retail Banking Platform containing Ledgers, Customers, and Accounts.

### Key Learnings
- **Double Entry Ledger Fidelity**: The Database uses strict arithmetic `CHECK` constraints to ensure a ledger transaction cannot be created with unbalanced debits/credits.
- **F# Event Sourcing**: The Application tier implements pure F# DDD commands (e.g., `Deposit`, `Transfer`). 
- **The Synergy**: The F# code does not need to define `type PositiveAmount = ...` because CanonFlow translates the DB `CHECK (amount > 0)` directly to the API boundary.
