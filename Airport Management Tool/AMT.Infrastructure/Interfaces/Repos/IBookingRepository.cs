using System;
using AMT.Domain.Models;

namespace AMT.Infrastructure.Interfaces.Repos;

public interface IBookingRepository : IGenericRepository<Booking>
{
    bool BookingExistsForTicket(int ticketId);
    bool BookingExists(int bookingId);
    Booking? GetByConfirmationCode(string confirmationCode);
}
