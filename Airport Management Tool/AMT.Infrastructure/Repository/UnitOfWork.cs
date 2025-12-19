using System;
using AMT.Infrastructure.Data;
using AMT.Infrastructure.Interfaces;
using AMT.Infrastructure.Interfaces.Repos;

namespace AMT.Infrastructure.Repository;

public sealed class UnitOfWork(
    AirportManagementContext ctx,
    IAirlineRepository airlines,
    IAirportRepository airports,
    IAircraftRepository aircraft,
    IFlightRepository flights,
    IFlightScheduleRepository flightSchedules,
    ITicketRepository tickets,
    IGateRepository gates,
    IBookingRepository bookings) : IUnitOfWork
{
    private readonly AirportManagementContext _ctx = ctx;

    public IAirlineRepository Airlines { get; } = airlines;
    public IAirportRepository Airports { get; } = airports;
    public IAircraftRepository Aircraft { get; } = aircraft;
    public IFlightRepository Flights { get; } = flights;
    public IFlightScheduleRepository FlightSchedules { get; } = flightSchedules;
    public ITicketRepository Tickets { get; } = tickets;
    public IBookingRepository Bookings { get; } = bookings;

    public IGateRepository Gates { get; } = gates;

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _ctx.SaveChangesAsync(ct);
      public ValueTask DisposeAsync() => _ctx.DisposeAsync();
}
