using System;

namespace AMT.Application.Services.Interfaces;

public interface IScheduleService
{

}//TODO - AirlineService / AirportService / GateService / AircraftService: simple CRUD + list for lookup data.
//   - FlightService: create/update/activate/deactivate flights; query by date/airline/route; enforce origin≠destination, flight number format, active status rules.
//   - FlightScheduleService: create/update/cancel schedules; validate arrival > departure, gate overlap prevention, assigned aircraft availability.
//   - TicketService: CRUD tickets; check availability; enforce positive pricing, seat inventory consistency; adjust inventory when bookings change.
//   - BookingService: create/get/cancel bookings; generate/validate confirmation codes; enforce no overbooking and status rules; integrate with TicketService for inventory adjustments.
//   - ScheduleImportService (bulk JSON upload): validate file size/row count, per-row validation, transactional per row, return mixed success/failure results.
//   - StatsService (staff): basic counts/availability summaries for dashboard endpoints.
