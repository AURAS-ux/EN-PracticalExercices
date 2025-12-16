using System;
using AMT.Domain.Models;
using AMT.Infrastructure.Data;
using AMT.Infrastructure.Exceptions;
using AMT.Infrastructure.Interfaces.Repos;
using AMT.Infrastructure.Mappers;

namespace AMT.Infrastructure.Repository;

public class AirlineRepository(AirportManagementContext context) : IAirlineRepository //TODO : Implement Repository
{
    public Task AddAsync(Airline entity)
    {
        context.Airlines.AddAsync(AirlineMap.ToEntity(entity));
        return context.SaveChangesAsync();
    }

    public Task DeleteAsync(int id)
    {
        var entity = context.Airlines.Find(id);
        if (entity != null)
        {
            context.Airlines.Remove(entity);
            return context.SaveChangesAsync();
        }
        else
        {
            throw new GenericNotFound<Airline, int>(id);
        }
    }

    public IEnumerable<Airline> GetAll()
    {
        return context.Airlines
            .Select(AirlineMap.ToDomain);
    }

    public Airline? GetById(int id)
    {
        return context.Airlines
        .Where(airline => airline.Id == id)
        .Select(AirlineMap.ToDomain)
        .FirstOrDefault();
    }

    public Task UpdateAsync(Airline entity)
    {
        var oldEntity = context.Airlines.Find(entity.Id);
        if (oldEntity != null)
        {
            context.Airlines.Update(AirlineMap.ToEntity(entity));
            return context.SaveChangesAsync();
        }
        else
        {
            throw new GenericNotFound<Airline, int>(entity.Id);
        }
    }
}
