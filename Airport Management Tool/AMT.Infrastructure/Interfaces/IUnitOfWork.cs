using System;
using AMT.Infrastructure.Interfaces.Repos;

namespace AMT.Infrastructure.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{ 
    IAirlineRepository Airlines { get; }
    IAirportRepository Airports { get; }
    IAircraftRepository Aircraft { get; }
    IFlightRepository Flights { get; }
    IGateRepository Gates { get; }
    IFlightScheduleRepository FlightSchedules { get; }
    ITicketRepository Tickets { get; }
    IBookingRepository Bookings { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
