# Codex Review v3

| Severity | Missing feature | Component |
| --- | --- | --- |
| High | Airport management endpoints cannot work because `AirportService.CreateAirportAsync`/`GetAirportById` are `NotImplementedException`, so staff cannot create or fetch airports. | AMT.Application/AirportService, api/airports |
| High | Booking retrieval/cancellation operate on numeric IDs and physically delete rows; no endpoint uses the confirmation code or marks bookings cancelled/restores inventory, so "cancel booking" use case and confirmation lookup are unavailable. | BookingController & BookingService |
| High | Gate overlap rule is not enforced—`IsScheduleConflictForGate` only checks identical timestamps and ignores arrival/departure windows, so overlapping schedules at the same gate are allowed. | ScheduleService, FlightScheduleRepository |
| Medium | Flight search/listing by date/airline/route is missing; FlightsController only offers CRUD and list-all, so client flight browsing use case is not covered. | FlightsController/FlightService |
| Medium | Pricing/capacity validation incomplete: Ticket validation omits positive price/currency/seat checks; booking only compares against ticket inventory and never aircraft capacity, so overbooking and negative prices slip through. | TicketService, TicketValidator, BookingService |
| Medium | Bulk schedule import aggregates results into a single success/fail entry, lacks per-row validation feedback and 1000-row cap, and always returns 201/207 without record-level statuses. | ScheduleService.BulkCreateSchedulesAsync, ScheduleController |
| Medium | Controllers return domain entities and ad-hoc error objects instead of DTO-based responses/problem details as required. | API controllers |
| High | No xUnit/Moq test project or coverage toward the required ~70%, leaving core booking/flight/import rules unverified. | Solution-wide |
