using System;
using AMT.Domain.Models;
using AMT.Infrastructure.Data;
using AMT.Infrastructure.Exceptions;
using AMT.Infrastructure.Interfaces.Repos;
using AMT.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace AMT.Infrastructure.Repository;

public class TicketRepository(AirportManagementContext context) : ITicketRepository
{
    public async Task AddAsync(Ticket entity)
    {
        await context.Tickets.AddAsync(TicketMap.ToEntity(entity));
    }

    public void Delete(int id)
    {
        var entity = context.Tickets.Find(id);
        if (entity != null)
        {
            context.Tickets.Remove(entity);
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

    public Ticket? GetByFlightId(int flightId)
    {
        return context.Tickets
            .Where(t => t.FlightId == flightId)
            .Select(TicketMap.ToDomain)
            .FirstOrDefault();
    }

    public Ticket? GetById(int id)
    {
        return context.Tickets
            .Include(t => t.Flight)
                .ThenInclude(f => f.Airline)
            .Include(t => t.Flight)
                .ThenInclude(f => f.OriginAirport)
            .Include(t => t.Flight)
                .ThenInclude(f => f.DestinationAirport)
            .Include(t => t.Flight)
                .ThenInclude(f => f.DefaultAircraft)
                    .ThenInclude(a => a!.OwnedByAirline)
            .Where(t => t.Id == id)
            .Select(TicketMap.ToDomain)
            .FirstOrDefault();
    }

    public Ticket? GetTicketByBookingId(int bookingId)
    {
        return context.Tickets
            .Include(t => t.Flight)
                .ThenInclude(f => f.Airline)
            .Include(t => t.Flight)
                .ThenInclude(f => f.OriginAirport)
            .Include(t => t.Flight)
                .ThenInclude(f => f.DestinationAirport)
            .Include(t => t.Flight)
                .ThenInclude(f => f.DefaultAircraft)
                    .ThenInclude(a => a!.OwnedByAirline)
            .Where(t => t.Bookings.Any(b => b.Id == bookingId))
            .Select(TicketMap.ToDomain)
            .FirstOrDefault();
    }

    public int? GetTicketIdByFlightId(int flightId)
    {
        var ticket = context.Tickets.FirstOrDefault(t => t.FlightId == flightId);
        return ticket?.Id;
    }

    public void Update(Ticket entity)
    {
        var oldEntity = context.Tickets.Find(entity.Id);
        if (oldEntity != null)
        {
            oldEntity.FlightId = entity.Flight.Id;
            oldEntity.FareClass = entity.FareClass;
            oldEntity.BasePrice = entity.BasePrice;
            oldEntity.Taxes = entity.Taxes;
            oldEntity.Currency = entity.Currency;
            oldEntity.IsRefundable = entity.IsRefundable;
            oldEntity.SeatInventory = entity.SeatInventory;
        }
        else
        {
            throw new GenericNotFound<Ticket,int>(entity.Id);
        }
    }
}
