using System;
using AMT.Domain.Models;
using AMT.Infrastructure.Data;
using AMT.Infrastructure.Exceptions;
using AMT.Infrastructure.Interfaces.Repos;
using AMT.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace AMT.Infrastructure.Repository;

public class BookingRepository(AirportManagementContext context) : IBookingRepository
{
    public async Task AddAsync(Booking entity)
    {
        await context.Bookings.AddAsync(BookingMap.ToEntity(entity));
    }

    public bool BookingExists(int bookingId)
    {
        return context.Bookings.Any(b => b.Id == bookingId);
    }

    public bool BookingExistsForTicket(int ticketId)
    {
        return context.Bookings.Any(b => b.TicketId == ticketId);
    }

    public void Delete(int id)
    {
        var entity = context.Bookings.Find(id);
        if (entity != null)
        {
            context.Bookings.Remove(entity);
        }
        else
        {
            throw new GenericNotFound<Booking, int>(id);
        }
    }

    public IEnumerable<Booking> GetAll()
    {
        return context.Bookings.Select(BookingMap.ToDomain);
    }

    public Booking? GetByConfirmationCode(string confirmationCode)
    {
        return context.Bookings
        .Include(b => b.Ticket)
        .Include(b => b.Flight)
        .Select(BookingMap.ToDomain)
        .FirstOrDefault(b => b.ConfirmationCode == confirmationCode);
    }

    public Booking? GetById(int id)
    {
        return context.Bookings
        .Include(b => b.Ticket)
        .Include(b => b.Flight)
        .Select(BookingMap.ToDomain)
        .FirstOrDefault(b => b.Id == id);
    }

    public void Update(Booking entity)
    {
        var oldEntity = context.Bookings.Find(entity.Id);
        if (oldEntity != null)
        {
            context.Entry(oldEntity).CurrentValues.SetValues(BookingMap.ToEntity(entity));
        }
        else
        {
            throw new GenericNotFound<Booking, int>(entity.Id);
        }
    }
}
