using System;
using AMT.Domain.Models;
using AMT.Infrastructure.Data;
using AMT.Infrastructure.Exceptions;
using AMT.Infrastructure.Interfaces.Repos;
using AMT.Infrastructure.Mappers;

namespace AMT.Infrastructure.Repository;

public class GateRepository(AirportManagementContext context) : IGateRepository
{
    public async Task AddAsync(Gate entity)
    {
        await context.Gates.AddAsync(GateMap.ToEntity(entity));
    }

    public void Delete(int id)
    {
        var entity = context.Gates.Find(id);
        if (entity != null)
        {
            context.Gates.Remove(entity);
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

    public void Update(Gate entity)
    {
        var oldEntity = context.Gates.Find(entity.Id);
        if (oldEntity != null)
        {
            context.Gates.Update(GateMap.ToEntity(entity));
        }
        else
        {
            throw new GenericNotFound<Gate,int>(entity.Id);
        }
    }
}
