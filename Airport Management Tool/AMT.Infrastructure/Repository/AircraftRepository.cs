using System;
using AMT.Domain.Models;
using AMT.Infrastructure.Data;
using AMT.Infrastructure.Exceptions;
using AMT.Infrastructure.Interfaces.Repos;
using AMT.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace AMT.Infrastructure.Repository;

public class AircraftRepository(AirportManagementContext context) : IAircraftRepository
{
    public async Task AddAsync(Aircraft entity)
    {
        await context.Aircraft.AddAsync(AircraftMap.ToEntity(entity));
    }

    public void Delete(int id)
    {
        var entity = context.Aircraft.Find(id);
        if (entity != null)
        {
            context.Aircraft.Remove(entity);
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
        .Include(aircraft => aircraft.OwnedByAirline)
        .Where(aircraft => aircraft.Id == id)
        .Select(AircraftMap.ToDomain)
        .FirstOrDefault();
    }

    public Aircraft? GetByTailNumber(string tailNumber)
    {
        return context.Aircraft
        .Where(aircraft => aircraft.TailNumber == tailNumber)
        .Select(AircraftMap.ToDomain)
        .FirstOrDefault();
    }

    public void Update(Aircraft entity)
    {
        var oldEntity = context.Aircraft.Find(entity.Id);
        if (oldEntity != null)
        {
            context.Aircraft.Update(AircraftMap.ToEntity(entity));
        }
        else
        {
            throw new GenericNotFound<Aircraft, int>(entity.Id);
        }
    }
}
