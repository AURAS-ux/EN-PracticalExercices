using System;
using AMT.Domain.Models;
using AMT.Infrastructure.Data;
using AMT.Infrastructure.Exceptions;
using AMT.Infrastructure.Interfaces.Repos;
using AMT.Infrastructure.Mappers;

namespace AMT.Infrastructure.Repository;

public class AircraftRepository(AirportManagementContext context) : IAircraftRepository
{
    public Task AddAsync(Aircraft entity)
    {
        context.Aircraft.AddAsync(AircraftMap.ToEntity(entity));
        return context.SaveChangesAsync();
    }

    public Task DeleteAsync(int id)
    {
        var entity = context.Aircraft.Find(id);
        if (entity != null)
        {
            context.Aircraft.Remove(entity);
            return context.SaveChangesAsync();
        }
        else
        {
            throw new GenericNotFound<Aircraft, int>(id);
        }
    }

    public IEnumerable<Aircraft> GetAll()
    {
        return context.Aircraft
            .Select(AircraftMap.ToDomain);
    }

    public Aircraft? GetById(int id)
    {
        return context.Aircraft
        .Where(aircraft => aircraft.Id == id)
        .Select(AircraftMap.ToDomain)
        .FirstOrDefault();
    }

    public Task UpdateAsync(Aircraft entity)
    {
        var oldEntity = context.Aircraft.Find(entity.Id);
        if (oldEntity != null)
        {
            context.Aircraft.Update(AircraftMap.ToEntity(entity));
            return context.SaveChangesAsync();
        }
        else
        {
            throw new GenericNotFound<Aircraft, int>(entity.Id);
        }
    }
}
