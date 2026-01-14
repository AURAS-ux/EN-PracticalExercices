using System;
using AMT.Application.Dtos;
using AMT.Domain.Models;
using AMT.Domain.Utils;

namespace AMT.Application.Services.Interfaces;

public interface ITicketService
{
    Result<Ticket, Exception> GetTicketByFlightId(int flightId);
    Task<Result<Ticket, Exception>> CreateTicket(CreateTicketDto ticket);
    Task<Result<Ticket, Exception>> UpdateTicketInventory(UpdateTicketInventoryDto updateTicketInventoryDto);
    Task<Result<string, Exception>> DeleteTicketAsync(int ticketId);
    Result<IEnumerable<Ticket>, Exception> GetAllTickets();
}
