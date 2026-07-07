# A5-pg-version-skew  [TIER 2 — real, but sequenced after Tier 1]

pg_get_constraintdef deparse output differs across PG 14/16/17. The gauntlet's ground truth moves with version. Run the gauntlet against a matrix of PG containers; classify deparse-grammar differences.

Do NOT build before Tier 1 holds. Attacking this early is the meta-trap:
sieging the interesting wall while a load-bearing one stands unverified.
