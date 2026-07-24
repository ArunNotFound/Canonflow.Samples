import { validate_billing_total_amount, validate_patient_gender, validate_patient_blood_group, validate_visit_type } from './validators';

describe('HospitalCore Validators', () => {
    test('billing total amount', () => {
        expect(validate_billing_total_amount({ total_amount: 100 })).toBe(true);
        expect(validate_billing_total_amount({ total_amount: -10 })).toBe(false);
    });

    test('patient gender', () => {
        expect(validate_patient_gender({ gender: "M" })).toBe(true);
        expect(validate_patient_gender({ gender: 'ALIEN' })).toBe(false);
    });

    test('patient blood group', () => {
        expect(validate_patient_blood_group({ blood_group: 'O+' })).toBe(true);
        expect(validate_patient_blood_group({ blood_group: 'XYZ' })).toBe(false);
    });

    test('visit type', () => {
        expect(validate_visit_type({ type: 'OUTPATIENT' })).toBe(true);
        expect(validate_visit_type({ type: 'INPATIENT' })).toBe(true);
        expect(validate_visit_type({ type: 'UNKNOWN' })).toBe(false);
    });
});
