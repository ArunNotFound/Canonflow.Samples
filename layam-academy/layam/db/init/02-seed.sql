INSERT INTO gurus (full_name, email, years_experience, specialization) VALUES
 ('Vidwan T. Raman','raman@layam.example',28,'vocal'),
 ('Smt. K. Bhairavi','bhairavi@layam.example',17,'violin'),
 ('Sri M. Laya','laya@layam.example',22,'mridangam');
INSERT INTO students (full_name,email,phone,age) VALUES
 ('Ananya S','ananya@ex.example','9876543210',12),
 ('Karthik V','karthik@ex.example','9876543211',34),
 ('Meenakshi R','meena@ex.example','9876543212',8),
 ('Devan P','devan@ex.example','9876543213',61);
INSERT INTO batches (guru_id,raga_focus,level,capacity,fee_monthly) VALUES
 (1,'Mayamalavagowla',1,10,1500.00),
 (1,'Shankarabharanam',4,8,3200.00),
 (2,'Kalyani',6,6,5400.00),
 (3,'Adi tala intensives',3,12,2800.00);
INSERT INTO enrollments (student_id,batch_id,discount_pct) VALUES
 (1,1,0),(3,1,25.0),(2,2,10.0),(4,3,0),(2,4,0);
INSERT INTO exams (student_id,batch_id,marks,theory_marks,practical_marks) VALUES
 (1,1,82,30,52),(2,2,91,40,51),(4,3,67,25,42);
INSERT INTO payments (student_id,batch_id,amount,method) VALUES
 (1,1,1500.00,'upi'),(3,1,1125.00,'cash'),(2,2,2880.00,'card');
