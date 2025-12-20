using System;
using AMT.Application.Dtos;
using AMT.Application.Services.Interfaces;
using AMT.Domain.Models;
using AMT.Domain.Utils;
using AMT.Infrastructure.Exceptions;
using AMT.Infrastructure.Interfaces;
using FluentValidation;
using Serilog;

namespace AMT.Application.Services;

public class BookingService(IUnitOfWork unitOfWork, ILogger logger,IValidator<Booking> validator) : IBookingService
{
    public async Task<Result<BookingCreatedDto, Exception>> CreateBookingAsync(CreateBookingDto bookingDto)
    {
        logger.Information("Creating booking with data: {@BookingDto}", bookingDto);
        var flightSchedule = unitOfWork.FlightSchedules.GetById(bookingDto.FlightScheduleId);
        if (flightSchedule == null)
        {
            logger.Warning("Flight schedule with ID {FlightScheduleId} not found.", bookingDto.FlightScheduleId);
            return Result<BookingCreatedDto, Exception>.Failure(
                new List<string> { "Flight schedule not found." },
                new List<Exception>
                {
                new GenericNotFound<FlightSchedule,int>(bookingDto.FlightScheduleId)
                },
                System.Net.HttpStatusCode.NotFound
            );
        }
        var ticket = unitOfWork.Tickets.GetById(bookingDto.TicketId);
        if (ticket == null)
        {
            logger.Warning("Ticket with ID {TicketId} not found.", bookingDto.TicketId);
            return Result<BookingCreatedDto, Exception>.Failure(
                new List<string> { "Ticket not found." },
                new List<Exception>
                {
                new GenericNotFound<Ticket,int>(bookingDto.TicketId)
                },
                System.Net.HttpStatusCode.NotFound
            );
        }

        var booking = new Booking
        {
            Flight = flightSchedule.Flight,
            Ticket = ticket,
            PassengerFullName = bookingDto.PassengerFullName,
            PassengerEmail = bookingDto.PassengerEmail,
            ConfirmationCode = GenerateConfirmationCode(),
            Quantity = bookingDto.Quantity,
            CreatedUtc = DateTime.UtcNow,
            Status = Booking.BookingStatus.ACTIVE
        };

        var validationResults = validator.Validate(booking);
        if (!validationResults.IsValid)
        {
            logger.Warning("Booking validation failed: {@Errors}", validationResults.Errors);
            return Result<BookingCreatedDto, Exception>.Failure(
                validationResults.Errors.Select(e => e.ErrorMessage).ToList(),
                validationResults.Errors.Select(e => new Infrastructure.Exceptions.ValidationException(e.ErrorMessage)).ToList<Exception>(),
                System.Net.HttpStatusCode.BadRequest
            );
        }

        await unitOfWork.Bookings.AddAsync(booking);
        ticket.SeatInventory -= bookingDto.Quantity;
        unitOfWork.Tickets.Update(ticket);
        await unitOfWork.SaveChangesAsync();
        logger.Information("Booking created successfully with confirmation code: {ConfirmationCode}", booking.ConfirmationCode);
        
        var savedBooking = unitOfWork.Bookings.GetByConfirmationCode(booking.ConfirmationCode);
        if (savedBooking == null)
        {
            logger.Error("Failed to retrieve the newly created booking with ConfirmationCode: {ConfirmationCode}", booking.ConfirmationCode);
            return Result<BookingCreatedDto, Exception>.Failure(
                new List<string> { "Failed to retrieve the newly created booking." },
                new List<Exception>
                {
                new Exception("Booking retrieval failed after creation.")
                },
                System.Net.HttpStatusCode.InternalServerError
            );
        }
        
        return Result<BookingCreatedDto, Exception>.Success(new BookingCreatedDto
        {
            BookingId = savedBooking.Id,
            ConfirmationCode = savedBooking.ConfirmationCode
        });
    }

    public async Task<Result<string, Exception>> DeleteBookingAsync(int bookingId)
    {
        logger.Information("Deleting booking with ID: {BookingId}", bookingId);
        if(!unitOfWork.Bookings.BookingExists(bookingId))
        {
            logger.Warning("Booking with ID {BookingId} not found.", bookingId);
            return Result<string, Exception>.Failure(
                new List<string> { "Booking not found." },
                new List<Exception>
                {
                new GenericNotFound<Booking,int>(bookingId)
                },
                System.Net.HttpStatusCode.NotFound
            );
        }
        unitOfWork.Bookings.Delete(bookingId);
        await unitOfWork.SaveChangesAsync();
        return Result<string, Exception>.Success("Booking deleted successfully.");
    }

    public Result<Booking, Exception> GetBookingByCode(int bookingCode)
    {
        logger.Information("Retrieving booking with confirmation code: {BookingCode}", bookingCode);
        var booking = unitOfWork.Bookings.GetById(bookingCode);
        if (booking == null)
        {
            logger.Warning("Booking with confirmation code {BookingCode} not found.", bookingCode);
            return Result<Booking, Exception>.Failure(
                ["Booking not found."],
                [new GenericNotFound<Booking,int>(bookingCode)],
                System.Net.HttpStatusCode.NotFound
            );
        }
        return Result<Booking, Exception>.Success(booking);
    }

    private string GenerateConfirmationCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 8)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
