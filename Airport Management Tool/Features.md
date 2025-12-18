# API Feature Scenarios

## Flights
| Done | Given | When | Then | Notes |
| --- | --- | --- | --- | --- |
| [ ] | Valid flight payload (airlineId present, originId != destinationId, flight number format) | POST `/api/flights` | 201 with flight; 409 on uniqueness conflicts | Validate origin/destination difference, flight number pattern, airline exists, optional default aircraft exists |
| [ ] | Flights exist | GET `/api/flights?date=...&airlineId=...&origin=...&destination=...` | Filtered, paged list | Support pagination, filtering by date/airline/route |
| [ ] | Existing flight | PUT `/api/flights/{id}` with valid changes | 200 with updated flight; 404 if not found | Preserve uniqueness; validate origin/destination and active flag rules |
| [ ] | Existing flight | PATCH `/api/flights/{id}/activate` or `/deactivate` | Active state toggles; 404 if not found | Return current state; enforce business rule checks before deactivate if needed |

## Schedules
| Done | Given | When | Then | Notes |
| --- | --- | --- | --- | --- |
| [ ] | Schedules exist | GET `/api/schedules/{id}` or list with date filters | Schedule details (times, gate, aircraft, status) | Support filtering by date; include gate/aircraft/status |
| [ ] | Valid schedule payload (arrival > departure, gate available) | POST `/api/schedules` | 201; 400 if gate overlap or invalid times | Enforce UTC, gate non-overlap, arrival after departure, optional aircraft availability |
| [ ] | JSON file <= 2MB with <= 1000 schedule rows | POST `/api/schedules/import` (file upload) | 201 if all pass; 207 with per-row success/errors; 400 if too large/invalid format | Per-row validation and persistence; limit size/count; partial success allowed |

## Tickets
| Done | Given | When | Then | Notes |
| --- | --- | --- | --- | --- |
| [ ] | Tickets exist | GET `/api/tickets?flightId=...` | Fare classes, prices, inventory, refundability | Support pagination/filtering; show availability |
| [ ] | Valid ticket payload (positive prices, inventory) | POST `/api/tickets` | 201; 400 on validation errors | Enforce BasePrice/Taxes >= 0, SeatInventory >= 0, currency code length, fare class length |
| [ ] | Existing ticket | PUT `/api/tickets/{id}` with valid changes | 200; 400 if invalid prices; 404 if missing | Prevent negative pricing/inventory; handle currency/fare class validation |
| [ ] | Ticket | DELETE `/api/tickets/{id}` | Deleted; 404 if missing; 409 if blocked by business rule | Block delete if active bookings; return 409 in that case |

## Bookings
| Done | Given | When | Then | Notes |
| --- | --- | --- | --- | --- |
| [ ] | Flight/ticket with sufficient inventory | POST `/api/bookings` with passenger info and quantity | 201 with confirmation code; inventory decreases; 400 on overbooking/invalid input | Validate email/name lengths, quantity > 0, confirmation code uniqueness, inventory check and decrement |
| [ ] | Existing booking | GET `/api/bookings/{id}` or `/api/bookings?confirmation=CODE` | Booking details; 404 if not found | Support lookup by id or confirmation code |
| [ ] | Active booking | POST `/api/bookings/{id}/cancel` | Status becomes cancelled; inventory restored; 404 if not found | Enforce idempotent cancel; do not over-increment inventory; return updated status |

## Stats (Staff)
| Done | Given | When | Then | Notes |
| --- | --- | --- | --- | --- |
| [ ] | Data exists | GET `/api/stats` | Summary counts (flights, active schedules, available seats, bookings, etc.) | Consider simple aggregates; no heavy joins; cache/paginate if needed |

## Validation & Errors
| Done | Given | When | Then | Notes |
| --- | --- | --- | --- | --- |
| [ ] | Invalid input (model validation fails) | Any endpoint | 400 with problem details | Use global validation handling; return field errors |
| [ ] | Missing resource | Requesting a non-existent item | 404 | Consistent problem details payload |
| [ ] | Server-side error | An unhandled exception occurs | Problem details response; logged | Global exception middleware; mask internal details |
