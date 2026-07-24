import { validate_account_cash_balance, validate_trade_order_side, validate_trade_order_quantity, validate_trade_order_status } from './validators';

describe('TradingCore Validators', () => {
    test('account cash balance', () => {
        expect(validate_account_cash_balance({ cash_balance: 100 })).toBe(true);
        expect(validate_account_cash_balance({ cash_balance: -10 })).toBe(false);
    });

    test('trade order side', () => {
        expect(validate_trade_order_side({ side: 'BUY' })).toBe(true);
        expect(validate_trade_order_side({ side: 'SELL' })).toBe(true);
        expect(validate_trade_order_side({ side: 'HOLD' })).toBe(false);
    });

    test('trade order quantity', () => {
        expect(validate_trade_order_quantity({ quantity: 100 })).toBe(true);
        expect(validate_trade_order_quantity({ quantity: 0 })).toBe(false);
    });

    test('trade order status', () => {
        expect(validate_trade_order_status({ status: 'PENDING' })).toBe(true);
        expect(validate_trade_order_status({ status: 'FILLED' })).toBe(true);
        expect(validate_trade_order_status({ status: 'DRAFT' })).toBe(false);
    });
});
