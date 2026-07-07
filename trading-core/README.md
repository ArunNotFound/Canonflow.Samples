# Trading Brokerage System

This sample demonstrates the perfect **Hand-in-Hand** split between Database Limits and Application Logic.

### Key Learnings
- **DB (The Physics)**: Evaluates strict limits like `quantity > 0` and `cash_balance >= 0`.
- **F# App (The Business)**: Evaluates the **Pattern Day Trader (PDT) Rule** (requires historical aggregate analysis) and **Market Hours** (requires dynamic timezones).
- **The Verdict**: The Database cannot easily evaluate timezones or historical window counts. The App should not waste time parsing simple `> 0` checks. CanonFlow perfectly bridges the two, resulting in the highest Power vs. Cost ratio.
