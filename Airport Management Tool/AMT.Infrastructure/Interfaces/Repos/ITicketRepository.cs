using System;
using AMT.Domain.Models;

namespace AMT.Infrastructure.Interfaces.Repos;

public interface ITicketRepository : IGenericRepository<Ticket>
{
    Ticket? GetByFlightId(int flightId);
    int? GetTicketIdByFlightId(int flightId);
    Ticket? GetTicketByBookingId(int bookingId);
}
