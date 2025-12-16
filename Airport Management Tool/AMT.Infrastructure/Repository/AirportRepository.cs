using System;
using AMT.Domain.Models;
using AMT.Infrastructure.Data;
using AMT.Infrastructure.Exceptions;
using AMT.Infrastructure.Interfaces.Repos;
using AMT.Infrastructure.Mappers;

namespace AMT.Infrastructure.Repository;

public class AirportRepository(AirportManagementContext context) : IAirportRepository
{
    public Task AddAsync(Airport entity)
    {
        context.Airports.AddAsync(AirportMap.ToEntity(entity));
        return context.SaveChangesAsync();
    }

    public Task DeleteAsync(int id)
    {
        var entity = context.Airports.Find(id);
        if (entity != null)
        {
            context.Airports.Remove(entity);
            return context.SaveChangesAsync();
        }
        else
        {
            throw new GenericNotFound<Airport, int>(id);
        }
    }

    public IEnumerable<Airport> GetAll()
    {
        return context.Airports.Select(AirportMap.ToDomain);
    }

    public Airport? GetById(int id)
    {
        return context.Airports
            .Where(a => a.Id == id)
            .Select(AirportMap.ToDomain)
            .FirstOrDefault();
    }

    public Task UpdateAsync(Airport entity)
    {
        var oldEntity = context.Airports.Find(entity.Id);
        if (oldEntity != null)
        {
            context.Airports.Update(AirportMap.ToEntity(entity));
            return context.SaveChangesAsync();
        }
        else
        {
            throw new GenericNotFound<Airport, int>(entity.Id);
        }
    }
}
