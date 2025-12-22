using System;
using AMT.Domain.Models;

namespace AMT.Infrastructure.Interfaces.Repos;

public interface IFlightScheduleRepository : IGenericRepository<FlightSchedule>
{
    int GetFlightSchedulesCountForDate(DateTime departureTimeUtc);
    IEnumerable<FlightSchedule> FilterFlightSchedules(string? origin, string? destination, DateTime? date);
    bool IsScheduleConflictForGate(int gateId, DateTime time);
    bool IsGateOverlapped(DateTime departureTimeUtc);
}
