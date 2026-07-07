# Airline Reservation System

This sample highlights the extreme difficulty of **Interconnected Rules** in Enterprise architecture.

### Key Learnings
- **Composite Foreign Keys**: The Database effortlessly handles a rule like "A booked seat must exist on the specific aircraft assigned to the booked flight" using overlapping composite keys.
- **DDD Performance Drop**: Enforcing this rule in pure DDD requires fetching three massive aggregates (Flight, Aircraft, Booking) into memory just to check a seat.
- **The CanonFlow Golden Rule**: This sample proves that interconnected structural bounds should always be DB-First Nouns, leaving the DDD Verbs unburdened by graph-fetching.
