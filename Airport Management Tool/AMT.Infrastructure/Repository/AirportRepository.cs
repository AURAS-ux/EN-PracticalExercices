using System;
using AMT.Domain.Models;
using AMT.Infrastructure.Data;
using AMT.Infrastructure.Exceptions;
using AMT.Infrastructure.Interfaces.Repos;
using AMT.Infrastructure.Mappers;

namespace AMT.Infrastructure.Repository;

public class AirportRepository(AirportManagementContext context) : IAirportRepository
{
    public async Task AddAsync(Airport entity)
    {
        await context.Airports.AddAsync(AirportMap.ToEntity(entity));
    }

    public void Delete(int id)
    {
        var entity = context.Airports.Find(id);
        if (entity != null)
        {
            context.Airports.Remove(entity);
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

    public Airport? GetByIataCode(string iataCode)
    {
        return context.Airports
            .Where(a => a.Iatacode == iataCode)
            .Select(AirportMap.ToDomain)
            .FirstOrDefault();
    }

    public Airport? GetById(int id)
    {
        return context.Airports
            .Where(a => a.Id == id)
            .Select(AirportMap.ToDomain)
            .FirstOrDefault();
    }

    public void Update(Airport entity)
    {
        var oldEntity = context.Airports.Find(entity.Id);
        if (oldEntity != null)
        {
            context.Airports.Update(AirportMap.ToEntity(entity));
        }
        else
        {
            throw new GenericNotFound<Airport, int>(entity.Id);
        }
    }
}
