using System;
using AMT.Domain.Models;
using AMT.Infrastructure.Data;
using AMT.Infrastructure.Exceptions;
using AMT.Infrastructure.Interfaces.Repos;
using AMT.Infrastructure.Mappers;

namespace AMT.Infrastructure.Repository;

public class BookingRepository(AirportManagementContext context) : IBookingRepository
{
    public Task AddAsync(Booking entity)
    {
        context.Bookings.AddAsync(BookingMap.ToEntity(entity));
        return context.SaveChangesAsync();
    }

    public Task DeleteAsync(int id)
    {
        var entity = context.Bookings.Find(id);
        if (entity != null)
        {
            context.Bookings.Remove(entity);
            return context.SaveChangesAsync();
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

    public Booking? GetById(int id)
    {
        return context.Bookings
        .Select(BookingMap.ToDomain)
        .FirstOrDefault(b => b.Id == id);
    }

    public Task UpdateAsync(Booking entity)
    {
        var oldEntity = context.Bookings.Find(entity.Id);
        if (oldEntity != null)
        {
            context.Entry(oldEntity).CurrentValues.SetValues(BookingMap.ToEntity(entity));
            return context.SaveChangesAsync();
        }
        else
        {
            throw new GenericNotFound<Booking, int>(entity.Id);
        }
    }
}
