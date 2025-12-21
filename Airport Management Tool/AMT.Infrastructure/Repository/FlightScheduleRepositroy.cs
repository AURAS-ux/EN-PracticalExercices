using System;
using AMT.Domain.Models;
using AMT.Infrastructure.Data;
using AMT.Infrastructure.Exceptions;
using AMT.Infrastructure.Interfaces.Repos;
using AMT.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace AMT.Infrastructure.Repository;

public class FlightScheduleRepositroy(AirportManagementContext context) : IFlightScheduleRepository
{
    public async Task AddAsync(FlightSchedule entity)
    {
        await context.FlightSchedules.AddAsync(FlightScheduleMap.ToEntity(entity));
    }

    public void Delete(int id)
    {
        var entity = context.FlightSchedules.Find(id);
        if (entity != null)
        {
            context.FlightSchedules.Remove(entity);
        }
        else
        {
            throw new GenericNotFound<FlightSchedule,int>(id);
        }
    }

    public IEnumerable<FlightSchedule> FilterFlightSchedules(string? origin, string? destination, DateTime? date)
    {
        return context.FlightSchedules
            .Include(fs => fs.Flight)
            .Include(fs => fs.Gate)
            .Include(fs => fs.AssignedAircraft)
            .Where(fs =>
                (string.IsNullOrEmpty(origin) || fs.Flight.OriginAirport.Iatacode.Equals(origin, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(destination) || fs.Flight.DestinationAirport.Iatacode.Equals(destination, StringComparison.OrdinalIgnoreCase)) &&
                (!date.HasValue || fs.ScheduledDepartureUtc.Date == date.Value.Date))
            .Select(FlightScheduleMap.ToDomain);
    }

    public IEnumerable<FlightSchedule> GetAll()
    {
        return context.FlightSchedules.Select(FlightScheduleMap.ToDomain);
    }

    public FlightSchedule? GetById(int id)
    {
        return context.FlightSchedules
            .Include(fs => fs.Flight)
            .Include(fs => fs.Gate)
            .Include(fs => fs.AssignedAircraft)
            .Where(fs => fs.Id == id)
            .Select(FlightScheduleMap.ToDomain)
            .FirstOrDefault();
    }

    public int GetFlightSchedulesCountForDate(DateTime departureTimeUtc)
    {
        return context.FlightSchedules
            .Count(fs => fs.ScheduledDepartureUtc.Date == departureTimeUtc.Date);
    }

    public bool IsScheduleConflictForGate(int gateId, DateTime time)
    {
        return context.FlightSchedules
            .Any(fs => fs.GateId == gateId && fs.ScheduledDepartureUtc == time);
    }

    public void Update(FlightSchedule entity)
    {
        var oldEntity = context.FlightSchedules.Find(entity.Id);
        if (oldEntity != null)
        {
            context.FlightSchedules.Update(FlightScheduleMap.ToEntity(entity));
        }
        else
        {
            throw new GenericNotFound<FlightSchedule,int>(entity.Id);
        }
    }
}
