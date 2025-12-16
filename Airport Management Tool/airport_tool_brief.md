# Web API Practice Brief — Airport Management Tool

## 1. Overview

Build a beginner-friendly ASP.NET Core 8 or 9 Web API for an Airport Management Tool used by:

- **Airport staff**: manage flights, aircraft, gates, and schedules; bulk‑import schedules from a JSON file.
- **Clients**: browse flight schedules, check ticket availability, and create bookings.

Use SQL Server with EF Core 8 or 9, a simple Repository + Unit of Work pattern, and xUnit + Moq for tests.

## 2. Learning Goals

- Create a multi-controller ASP.NET Core Web API (routing, model binding, validation).
- Configure EF Core DbContext, entities, relationships, migrations.
- Implement Repository + Unit of Work for basic CRUD.
- Handle file uploads; parse and validate JSON payloads.
- Implement DTOs, mapping, and simple error handling.
- Write unit tests with xUnit and Moq.
- Apply basic SQL indexing and query performance hygiene.

## 3. Functional & Non-Functional Requirements

### 3.1 Functional

- Authentication: omit full auth; expose endpoints openly.
- Controllers (min 4):
  - **FlightsController**: CRUD flights; list by date, airline, route.
  - **SchedulesController**: read schedule, bulk import.
  - **TicketsController**: ticket availability, CRUD.
  - **BookingsController**: create/get/cancel bookings.
- Bulk Import: JSON upload with per-record validation and results.
- Client Use Cases: search flights, view availability, create booking.
- Staff Use Cases: manage entities, stats endpoint.
- Validation: no gate overlaps; no overbooking; positive prices.
- Errors: problem details; 404 not found; 201 created.

### 3.2 Non-Functional

- .NET 8/9, EF Core 8/9, SQL Server 2019+.
- Async patterns, logging, Options pattern, performance indexing.
- DTOs only; global exception middleware.

## 4. Architecture & Patterns

Solution layout (multiple projects):

- **Domain**: entities.
- **Application**: DTOs, services, interfaces.
- **Infrastructure**: EF Core, repositories, UoW.
- **WebApi**: controllers, DI, pipeline.
- **Tests**: xUnit + Moq.

Patterns: Repository + UoW, Service Layer, DTO mapping, optional Specification.

## 5. Data Model

Entities: Airline, Airport, Gate, Aircraft, Flight, FlightSchedule, Ticket, Booking.

Includes business rules such as:

- FlightNumber format, origin ≠ destination.
- Arrival > Departure.
- Gate overlap prevention.
- Capacity enforcement.
- UTC timestamps.

Indexes:
- Unique IATA codes.
- Flight indexes.
- Schedule indexes.
- Booking confirmation code unique.

## 6. API Specification

### 6.1 Conventions
Base route `/api/`, DTOs, pagination, validation.

### 6.2 Endpoints
Detailed examples for Flights, Schedules (including import), Tickets, Bookings.

### 6.3 File Upload Import
JSON file, max 2MB, max 1000 rows, per-row transactions, response 201/207/400.

## 7. Unit Testing Requirements

Use xUnit, Moq.  
Test application services, controllers, repositories.  
Mock IFormFile for imports.  
Coverage ≥ 70%.

## 8. Acceptance Criteria

CRUD works, import works with mixed results, booking rules enforced, migrations correct, logs produced, tests green.

## 9. Evaluation Rubric (100 pts)

Breakdown: correctness, code quality, data model, testing, security, performance.

## 10. Extensions (Optional)

JWT auth, concurrency tokens, refund policy, CSV export, MediatR.

## 11. Deliverables & Constraints

Repo structure, Swagger enabled, migrations included, README with setup.

## 12. Example Payloads & Status Codes

Quick reference for bookings, import, ticket deletion.

## Reviewer Guidance

Look for layering, DTO boundaries, robust import behavior, correct indexes, strong business-rule tests.

