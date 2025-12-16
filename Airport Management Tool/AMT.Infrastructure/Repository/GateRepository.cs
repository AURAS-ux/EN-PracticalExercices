using System;
using AMT.Domain.Models;
using AMT.Infrastructure.Data;
using AMT.Infrastructure.Exceptions;
using AMT.Infrastructure.Interfaces.Repos;
using AMT.Infrastructure.Mappers;

namespace AMT.Infrastructure.Repository;

public class GateRepository(AirportManagementContext context) : IGateRepository
{
    public Task AddAsync(Gate entity)
    {
        context.Gates.AddAsync(GateMap.ToEntity(entity));
        return context.SaveChangesAsync();
    }

    public Task DeleteAsync(int id)
    {
        var entity = context.Gates.Find(id);
        if (entity != null)
        {
            context.Gates.Remove(entity);
            return context.SaveChangesAsync();
        }
        else
        {
            throw new GenericNotFound<Gate,int>(id);
        }
    }

    public IEnumerable<Gate> GetAll()
    {
        return context.Gates.Select(GateMap.ToDomain);
    }

    public Gate? GetById(int id)
    {
        return context.Gates
            .Where(g => g.Id == id)
            .Select(GateMap.ToDomain)
            .FirstOrDefault();
    }

    public Task UpdateAsync(Gate entity)
    {
        var oldEntity = context.Gates.Find(entity.Id);
        if (oldEntity != null)
        {
            context.Gates.Update(GateMap.ToEntity(entity));
            return context.SaveChangesAsync();
        }
        else
        {
            throw new GenericNotFound<Gate,int>(entity.Id);
        }
    }
}
