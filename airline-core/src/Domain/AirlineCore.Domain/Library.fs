namespace AirlineCore.Domain

open System

// ==========================================
// 1. Primitive Value Objects & Types
// ==========================================
type PassengerId = PassengerId of Guid
type FlightId = FlightId of Guid
type AircraftId = AircraftId of Guid
type BookingId = BookingId of Guid

type SeatNumber = private SeatNumber of string
module SeatNumber =
    let create (s: string) =
        if not (String.IsNullOrWhiteSpace(s)) then Ok (SeatNumber s)
        else Error "Seat number cannot be empty"
    let value (SeatNumber s) = s

type AirportCode = private AirportCode of string
module AirportCode =
    let create (s: string) =
        if s.Length = 3 then Ok (AirportCode s.ToUpper())
        else Error "Airport code must be exactly 3 characters"
    let value (AirportCode s) = s

// ==========================================
// 2. Interconnected Rules in DDD
// ==========================================
// Surprise: SQL handles interconnected rules using composite foreign keys effortlessly
// (e.g., Booking -> Flight(FlightId, AircraftId) & Booking -> Seat(AircraftId, SeatNumber)).
// In DDD, we have to fetch the aggregates and compare their internal properties manually
// before allowing the behavior to proceed.

type Seat = {
    SeatNumber: SeatNumber
    Class: string
}

type Aircraft = {
    Id: AircraftId
    TailNumber: string
    Seats: Seat list // Aggregate root holding its seats
}

type Flight = {
    Id: FlightId
    AssignedAircraftId: AircraftId
    Origin: AirportCode
    Destination: AirportCode
    Status: string
}

type Booking = {
    Id: BookingId
    PassengerId: PassengerId
    FlightId: FlightId
    SeatNumber: SeatNumber
    Status: string
}

module BookingBehavior =
    type Command =
        | BookSeat of PassengerId * FlightId * AircraftId * string

    type Event =
        | SeatBooked of BookingId
        | BookingFailed of reason: string

    // DDD requires us to pass in all aggregates to enforce the interconnected rules.
    let execute (cmd: Command) (flight: Flight) (aircraft: Aircraft) : Result<Event list, string> =
        match cmd with
        | BookSeat (passengerId, targetFlightId, targetAircraftId, seatStr) ->
            
            // Interconnected Rule 1: The flight must match the assigned aircraft
            if flight.Id <> targetFlightId then
                Error "Flight mismatch"
            elif flight.AssignedAircraftId <> targetAircraftId then
                Error "The assigned aircraft for this flight does not match the booking request"
            
            // Interconnected Rule 2: The aircraft must actually have this seat
            elif aircraft.Id <> targetAircraftId then
                Error "Aircraft aggregate mismatch"
            else
                match SeatNumber.create seatStr with
                | Error e -> Error e
                | Ok validSeat ->
                    let seatExists = aircraft.Seats |> List.exists (fun s -> s.SeatNumber = validSeat)
                    if not seatExists then
                        Error "Seat does not exist on this specific aircraft model"
                    else
                        // Success
                        Ok [ SeatBooked (BookingId (Guid.NewGuid())) ]
