using System;
using AMT.Domain.Models;
using AMT.Infrastructure.Data;
using AMT.Infrastructure.Exceptions;
using AMT.Infrastructure.Interfaces.Repos;
using AMT.Infrastructure.Mappers;

namespace AMT.Infrastructure.Repository;

public class FlightScheduleRepositroy(AirportManagementContext context) : IFlightScheduleRepository
{
    public Task AddAsync(FlightSchedule entity)
    {
        context.FlightSchedules.AddAsync(FlightScheduleMap.ToEntity(entity));
        return context.SaveChangesAsync();
    }

    public Task DeleteAsync(int id)
    {
        var entity = context.FlightSchedules.Find(id);
        if (entity != null)
        {
            context.FlightSchedules.Remove(entity);
            return context.SaveChangesAsync();
        }
        else
        {
            throw new GenericNotFound<FlightSchedule,int>(id);
        }
    }

    public IEnumerable<FlightSchedule> GetAll()
    {
        return context.FlightSchedules.Select(FlightScheduleMap.ToDomain);
    }

    public FlightSchedule? GetById(int id)
    {
        return context.FlightSchedules
            .Where(fs => fs.Id == id)
            .Select(FlightScheduleMap.ToDomain)
            .FirstOrDefault();
    }

    public Task UpdateAsync(FlightSchedule entity)
    {
        var oldEntity = context.FlightSchedules.Find(entity.Id);
        if (oldEntity != null)
        {
            context.FlightSchedules.Update(FlightScheduleMap.ToEntity(entity));
            return context.SaveChangesAsync();
        }
        else
        {
            throw new GenericNotFound<FlightSchedule,int>(entity.Id);
        }
    }
}
