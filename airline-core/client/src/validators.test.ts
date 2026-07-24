import { validate_aircraft_max_capacity, validate_crew_role, validate_loyalty_tier, validate_seat_class } from './validators';

describe('AirlineCore Validators', () => {
    test('aircraft max capacity', () => {
        expect(validate_aircraft_max_capacity({ max_capacity: 100 })).toBe(true);
        expect(validate_aircraft_max_capacity({ max_capacity: 0 })).toBe(false);
    });

    test('crew role', () => {
        expect(validate_crew_role({ role: 'PILOT' })).toBe(true);
        expect(validate_crew_role({ role: 'PASSENGER' })).toBe(false);
    });

    test('loyalty tier', () => {
        expect(validate_loyalty_tier({ tier: 'GOLD' })).toBe(true);
        expect(validate_loyalty_tier({ tier: 'RUST' })).toBe(false);
    });

    test('seat class', () => {
        expect(validate_seat_class({ class: 'ECONOMY' })).toBe(true);
        expect(validate_seat_class({ class: 'COACH' })).toBe(false);
    });
});
