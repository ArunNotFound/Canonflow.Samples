-- S7: bidirectional drift. Run after introspection, then `canonflow drift`.
-- Expected: EXACTLY TWO violations, opposite FieldClass directions.
-- (a) WIDENED: loan principal cap raised
ALTER TABLE loans DROP CONSTRAINT loan_principal_window;
ALTER TABLE loans ADD CONSTRAINT loan_principal_window
  CHECK (principal >= 1000 AND principal <= 750000);
-- (b) NARROWED (the dangerous direction — existing rows may violate):
ALTER TABLE deposits DROP CONSTRAINT deposit_min;
ALTER TABLE deposits ADD CONSTRAINT deposit_min CHECK (amount >= 1000);
