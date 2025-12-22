using System;
using System.Threading.Tasks;
using AMT.Application.Dtos;
using AMT.Application.Services.Interfaces;
using AMT.Domain.Models;
using AMT.Domain.Utils;
using AMT.Infrastructure.Interfaces;
using FluentValidation;
using Serilog;

namespace AMT.Application.Services;

public class TicketService(IUnitOfWork unitOfWork, ILogger logger, IValidator<Ticket> validator) : ITicketService
{
    public async Task<Result<Ticket, Exception>> CreateTicket(CreateTicketDto createTicketDto)
    {
        logger.Information("Creating ticket for flight ID: {FlightId}", createTicketDto.FlightId);
        var flight = unitOfWork.Flights.GetById(createTicketDto.FlightId);
        if (flight == null)
        {
            var errorMessage = $"Flight with ID {createTicketDto.FlightId} not found.";
            logger.Error(errorMessage);
            return Result<Ticket, Exception>.Failure(
                new List<string> { errorMessage },
                [new KeyNotFoundException(errorMessage)],
                System.Net.HttpStatusCode.NotFound);
        }
        var newTicket = new Ticket
        {
            Flight = flight,
            FareClass = createTicketDto.FareClass,
            BasePrice = createTicketDto.BasePrice,
            Taxes = createTicketDto.Taxes,
            Currency = createTicketDto.Currency,
            SeatInventory = createTicketDto.SeatInventory,
            IsRefundable = createTicketDto.IsRefoundable
        };

        var validationResult = validator.Validate(newTicket);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            logger.Error("Ticket validation failed: {Errors}", string.Join(", ", errors));
            return Result<Ticket, Exception>.Failure(
                errors,
                validationResult.Errors.Select(e => new Infrastructure.Exceptions.ValidationException(e.ErrorMessage)).ToList<Exception>(),
                System.Net.HttpStatusCode.BadRequest);
        }

        await unitOfWork.Tickets.AddAsync(newTicket);
        await unitOfWork.SaveChangesAsync();
        logger.Information("Ticket created successfully with ID: {TicketId}", newTicket.Id);
        return Result<Ticket, Exception>.Success(newTicket);
    }

    public async Task<Result<string, Exception>> DeleteTicketAsync(int ticketId)
    {
        logger.Information("Checking for existing bookings for ticket ID: {TicketId}", ticketId);
        if (unitOfWork.Bookings.BookingExistsForTicket(ticketId))
        {
            var errorMessage = $"Cannot delete ticket ID {ticketId} as it has associated bookings.";
            logger.Error(errorMessage);
            return Result<string, Exception>.Failure(
                new List<string> { errorMessage },
                [new InvalidOperationException(errorMessage)],
                System.Net.HttpStatusCode.BadRequest);
        }
        logger.Information("Deleting ticket ID: {TicketId}", ticketId);
        unitOfWork.Tickets.Delete(ticketId);
        await unitOfWork.SaveChangesAsync();
        logger.Information("Ticket ID: {TicketId} deleted successfully", ticketId);
        return Result<string, Exception>.Success($"Ticket ID {ticketId} deleted successfully.");
    }

    public Result<IEnumerable<Ticket>, Exception> GetAllTickets()
    {
        var tickets = unitOfWork.Tickets.GetAll();
        return Result<IEnumerable<Ticket>, Exception>.Success(tickets);
    }

    public Result<Ticket, Exception> GetTicketByFlightId(int flightId)
    {
        var ticket = unitOfWork.Tickets.GetByFlightId(flightId);
        if (ticket == null)
        {
            var errorMessage = $"Ticket for flight ID {flightId} not found.";
            logger.Error(errorMessage);
            return Result<Ticket, Exception>.Failure(
                new List<string> { errorMessage },
                [new KeyNotFoundException(errorMessage)],
                System.Net.HttpStatusCode.NotFound);
        }
        return Result<Ticket, Exception>.Success(ticket);
    }

    public async Task<Result<Ticket, Exception>> UpdateTicketInventory(UpdateTicketInventoryDto updateTicketInventoryDto)
    {
        var existingTicket = unitOfWork.Tickets.GetById(updateTicketInventoryDto.TicketId);
        if (existingTicket == null)
        {
            var errorMessage = $"Ticket with ID {updateTicketInventoryDto.TicketId} not found.";
            logger.Error(errorMessage);
            return Result<Ticket, Exception>.Failure(
                new List<string> { errorMessage },
                [new KeyNotFoundException(errorMessage)],
                System.Net.HttpStatusCode.NotFound);
        }
        existingTicket.SeatInventory = updateTicketInventoryDto.NewSeatInventory;
        unitOfWork.Tickets.Update(existingTicket);
        await unitOfWork.SaveChangesAsync();
        logger.Information("Ticket ID: {TicketId} inventory updated to {NewInventory}", existingTicket.Id, existingTicket.SeatInventory);
        return Result<Ticket, Exception>.Success(existingTicket);
    }
}
