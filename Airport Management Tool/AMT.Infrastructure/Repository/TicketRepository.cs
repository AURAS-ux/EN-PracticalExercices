using System;
using AMT.Domain.Models;
using AMT.Infrastructure.Data;
using AMT.Infrastructure.Exceptions;
using AMT.Infrastructure.Interfaces.Repos;
using AMT.Infrastructure.Mappers;

namespace AMT.Infrastructure.Repository;

public class TicketRepository(AirportManagementContext context) : ITicketRepository
{
    public Task AddAsync(Ticket entity)
    {
        context.Tickets.AddAsync(TicketMap.ToEntity(entity));
        return context.SaveChangesAsync();
    }

    public Task DeleteAsync(int id)
    {
        var entity = context.Tickets.Find(id);
        if (entity != null)
        {
            context.Tickets.Remove(entity);
            return context.SaveChangesAsync();
        }
        else
        {
            throw new GenericNotFound<Ticket,int>(id);
        }
    }

    public IEnumerable<Ticket> GetAll()
    {
        return context.Tickets.Select(TicketMap.ToDomain);
    }

    public Ticket? GetById(int id)
    {
        return context.Tickets
            .Where(t => t.Id == id)
            .Select(TicketMap.ToDomain)
            .FirstOrDefault();
    }

    public Task UpdateAsync(Ticket entity)
    {
        var oldEntity = context.Tickets.Find(entity.Id);
        if (oldEntity != null)
        {
            context.Tickets.Update(TicketMap.ToEntity(entity));
            return context.SaveChangesAsync();
        }
        else
        {
            throw new GenericNotFound<Ticket,int>(entity.Id);
        }
    }
}
