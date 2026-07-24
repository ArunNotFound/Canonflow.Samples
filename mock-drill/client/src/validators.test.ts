import { 
    validate_customers_age, 
    validate_customers_status, 
    validate_customers_credit_limit, 
    validate_orders_amount, 
    validate_orders_currency, 
    validate_orders_order_status 
} from "./validators";

describe("CanonFlow Transpiled Validators", () => {
    describe("Customers", () => {
        test("age validation", () => {
            expect(validate_customers_age({ age: 25 })).toBe(true);
            expect(validate_customers_age({ age: 18 })).toBe(true);
            expect(validate_customers_age({ age: 17 })).toBe(false);
            expect(validate_customers_age({ age: 120 })).toBe(false); // age < 120
            expect(validate_customers_age({ age: 119 })).toBe(true);
        });

        test("status validation", () => {
            expect(validate_customers_status({ status: "ACTIVE" })).toBe(true);
            expect(validate_customers_status({ status: "SUSPENDED" })).toBe(true);
            expect(validate_customers_status({ status: "CLOSED" })).toBe(true);
            expect(validate_customers_status({ status: "UNKNOWN" })).toBe(false);
            expect(validate_customers_status({ status: "active" })).toBe(false);
        });

        test("credit_limit validation", () => {
            expect(validate_customers_credit_limit({ credit_limit: 500 })).toBe(true);
            expect(validate_customers_credit_limit({ credit_limit: 0 })).toBe(true);
            expect(validate_customers_credit_limit({ credit_limit: 1000000 })).toBe(true);
            expect(validate_customers_credit_limit({ credit_limit: -1 })).toBe(false);
            expect(validate_customers_credit_limit({ credit_limit: 1000001 })).toBe(false);
        });
    });

    describe("Orders", () => {
        test("amount validation", () => {
            expect(validate_orders_amount({ amount: 100.5 })).toBe(true);
            expect(validate_orders_amount({ amount: 0.01 })).toBe(true);
            expect(validate_orders_amount({ amount: 0 })).toBe(false);
            expect(validate_orders_amount({ amount: -10 })).toBe(false);
        });

        test("currency validation", () => {
            expect(validate_orders_currency({ currency: "INR" })).toBe(true);
            expect(validate_orders_currency({ currency: "USD" })).toBe(true);
            expect(validate_orders_currency({ currency: "EUR" })).toBe(true);
            expect(validate_orders_currency({ currency: "GBP" })).toBe(false);
            expect(validate_orders_currency({ currency: "usd" })).toBe(false);
        });

        test("order_status validation", () => {
            expect(validate_orders_order_status({ order_status: "PLACED" })).toBe(true);
            expect(validate_orders_order_status({ order_status: "PAID" })).toBe(true);
            expect(validate_orders_order_status({ order_status: "CANCELLED" })).toBe(true);
            expect(validate_orders_order_status({ order_status: "PENDING" })).toBe(false);
        });
    });
});
