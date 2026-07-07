# Hospital Management System

This sample models the entire clinical flow from Patient Admission to Claim Adjudication.

### Key Learnings
- **Cross-Column Invariants**: Showcases CanonFlow decoding `CHECK (total_amount = patient_responsibility + insurance_responsibility)`.
- **Timeline Boundaries**: Showcases decoding `CHECK (completed_time IS NULL OR completed_time >= ordered_time)`.
- **The Boilerplate Destruction**: Writing F# refinement types for 20 massive clinical tables would require tens of thousands of lines of code. CanonFlow pushes these structural nouns to the DB and emits the validators automatically, saving immense token and developer cost.
