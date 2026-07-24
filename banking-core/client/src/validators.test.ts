import { validate_account_account_type, validate_customer_risk_rating, validate_transaction_txn_type, validate_charges_amount } from './validators';

describe('BankingCore Validators', () => {
    test('account type', () => {
        expect(validate_account_account_type({ account_type: 'SAVINGS' })).toBe(true);
        expect(validate_account_account_type({ account_type: 'INVALID' })).toBe(false);
    });

    test('customer risk rating', () => {
        expect(validate_customer_risk_rating({ risk_rating: 'HIGH' })).toBe(true);
        expect(validate_customer_risk_rating({ risk_rating: 'UNKNOWN' })).toBe(false);
    });

    test('transaction type', () => {
        expect(validate_transaction_txn_type({ txn_type: 'CREDIT' })).toBe(true);
        expect(validate_transaction_txn_type({ txn_type: 'DEPOSIT' })).toBe(false);
    });

    test('charges amount', () => {
        expect(validate_charges_amount({ amount: 10 })).toBe(true);
        expect(validate_charges_amount({ amount: 0 })).toBe(false);
    });
});
