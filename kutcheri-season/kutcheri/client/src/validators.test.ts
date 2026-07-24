import { validate_artists_form, validate_bookings_quantity, validate_kutcheris_status, validate_ticket_tiers_price } from './validators';

describe('Kutcheri Validators', () => {
    test('artists form', () => {
        expect(validate_artists_form({ form: 'Vocal' })).toBe(true);
        expect(validate_artists_form({ form: 'Guitar' })).toBe(false);
    });

    test('bookings quantity', () => {
        expect(validate_bookings_quantity({ quantity: 1 })).toBe(true);
        expect(validate_bookings_quantity({ quantity: 0 })).toBe(false);
    });

    test('kutcheris status', () => {
        expect(validate_kutcheris_status({ status: 'confirmed' })).toBe(true);
        expect(validate_kutcheris_status({ status: 'unknown' })).toBe(false);
    });

    test('ticket tiers price', () => {
        expect(validate_ticket_tiers_price({ price: 100 })).toBe(true);
        expect(validate_ticket_tiers_price({ price: -1 })).toBe(false);
    });
});
