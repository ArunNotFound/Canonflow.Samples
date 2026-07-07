INSERT INTO members (full_name,phone,age,guardian_member_id,share_balance,"riskGrade") VALUES
 ('Selvam K','9812345670',45,NULL,2500.00,'B'),
 ('Priya M','9812345671',33,NULL,1200.00,'A'),
 ('Arjun S','9812345672',17,1,500.00,'C');          -- minor + guardian: S2 satisfied via OR leg 2
INSERT INTO deposits (member_id,amount,opened_on,maturity_date,rate_pct) VALUES
 (1,10000.00,'2026-01-15','2027-01-15',7.50),
 (2,500.00,'2026-03-01','2026-09-01',5.25);
INSERT INTO loans (member_id,principal,tenure_months,interest_pct) VALUES
 (1,150000.00,36,14.50),(2,25000.00,12,17.00);
INSERT INTO guarantees (loan_id,guarantor_id,guarantor_share_pct) VALUES
 (1,2,50.0),
 (2,NULL,NULL);   -- S1 live: the row a naive proof says cannot exist
INSERT INTO ledger (member_id,ledger_adjustment,method) VALUES
 (1,-250.00,'upi'),(2,1000.00,'neft');
