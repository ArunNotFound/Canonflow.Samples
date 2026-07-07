-- SPECIMEN 2 (deliberate): drift. Run AFTER first introspection, then run
-- `canonflow drift`. Expected: exactly one DriftViolation on batches.fee_monthly.
ALTER TABLE batches DROP CONSTRAINT batch_fee_window;
ALTER TABLE batches ADD CONSTRAINT batch_fee_window
  CHECK (fee_monthly >= 500 AND fee_monthly <= 20000);
