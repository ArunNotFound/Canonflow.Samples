# CanonFlow Migration Engine Demo

This sample demonstrates CanonFlow's internal `--migrateto` engine.

### Key Learnings
- **Schema Evolution**: You specify Database V1 (current) and Database V2 (target).
- **Mathematical Diffing**: CanonFlow loads both databases into its `Lattice` AST, diffs the topologies in memory, and dynamically generates safe, transactional `BEGIN; ... COMMIT;` SQL scripts to bridge the gap.
- **The End of Flyway**: By treating the Database as the single source of truth, you don't write migrations. You just write the desired state, and CanonFlow builds the bridge.
