using System;
using AMT.Domain.Models;
using AMT.Infrastructure.Data;
using AMT.Infrastructure.Exceptions;
using AMT.Infrastructure.Interfaces.Repos;
using AMT.Infrastructure.Mappers;

namespace AMT.Infrastructure.Repository;

public class FlightRepository(AirportManagementContext context) : IFlightRepository
{
    public Task AddAsync(Flight entity)
    {
        context.AddAsync(FlightMap.ToEntity(entity));
        return context.SaveChangesAsync();
    }

    public Task DeleteAsync(int id)
    {
        var entity = context.Flights.Find(id);
        if (entity != null)
        {
            context.Flights.Remove(entity);
            return context.SaveChangesAsync();
        }
        else
        {
            throw new GenericNotFound<Flight, int>(id);
        }
    }

    public IEnumerable<Flight> GetAll()
    {
        return context.Flights.Select(FlightMap.ToDomain);
    }

    public Flight? GetById(int id)
    {
        return context.Flights
            .Where(f => f.Id == id)
            .Select(FlightMap.ToDomain)
            .FirstOrDefault();
    }

    public Task UpdateAsync(Flight entity)
    {
        var oldEntity = context.Flights.Find(entity.Id);
        if (oldEntity != null)
        {
            context.Entry(oldEntity).CurrentValues.SetValues(FlightMap.ToEntity(entity));
            return context.SaveChangesAsync();
        }
        else
        {
            throw new GenericNotFound<Flight, int>(entity.Id);
        }
    }
}
