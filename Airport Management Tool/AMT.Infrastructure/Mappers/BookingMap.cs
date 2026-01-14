using System;
using DomainAircraft = AMT.Domain.Models.Aircraft;
using DomainAirline = AMT.Domain.Models.Airline;
using DomainAirport = AMT.Domain.Models.Airport;
using DomainBooking = AMT.Domain.Models.Booking;
using DomainFlight = AMT.Domain.Models.Flight;
using DomainTicket = AMT.Domain.Models.Ticket;
using EntityBooking = AMT.Infrastructure.Entities.Booking;

namespace AMT.Infrastructure.Mappers;

public static class BookingMap
{
    public static DomainBooking ToDomain(EntityBooking entity)
    {
        var flight = entity.Flight != null
            ? FlightMap.ToDomain(entity.Flight)
            : new DomainFlight
            {
                Id = entity.FlightId,
                Airline = new DomainAirline { Id = 0, Iatacode = string.Empty, Name = string.Empty },
                FlightNumber = string.Empty,
                OriginAirport = new DomainAirport { Id = 0, IATACode = string.Empty, Name = string.Empty, City = string.Empty, Country = string.Empty, Timezone = string.Empty },
                DestinationAirport = new DomainAirport { Id = 0, IATACode = string.Empty, Name = string.Empty, City = string.Empty, Country = string.Empty, Timezone = string.Empty },
                DefaultAircraft = new DomainAircraft { Id = 0, TailNumber = string.Empty, Model = string.Empty, SeatCapacity = 0, OwnedByAirline = new DomainAirline { Id = 0, Iatacode = string.Empty, Name = string.Empty } },
                IsActive = entity.Flight?.IsActive ?? true
            };

        var ticket = entity.Ticket != null
            ? TicketMap.ToDomain(entity.Ticket)
            : new DomainTicket
            {
                Id = entity.TicketId,
                Flight = flight,
                FareClass = string.Empty,
                BasePrice = 0,
                Taxes = 0,
                Currency = string.Empty,
                IsRefundable = false,
                SeatInventory = 0
            };

        return new DomainBooking
        {
            Id = entity.Id,
            Flight = flight,
            Ticket = ticket,
            PassengerFullName = entity.PassagerFullName,
            PassengerEmail = entity.PassagerEmail,
            ConfirmationCode = entity.ConfirmationCode,
            Quantity = entity.Quantity,
            CreatedUtc = entity.CreatedUtc ?? DateTime.UtcNow,
            Status = entity.Status == 1 ? DomainBooking.BookingStatus.CANCELLED : DomainBooking.BookingStatus.ACTIVE
        };
    }

    public static EntityBooking ToEntity(DomainBooking model)
    {
        return new EntityBooking
        {
            Id = model.Id,
            FlightId = model.Flight.Id,
            TicketId = model.Ticket.Id,
            PassagerFullName = model.PassengerFullName,
            PassagerEmail = model.PassengerEmail,
            ConfirmationCode = model.ConfirmationCode,
            Quantity = model.Quantity,
            Status = model.Status == DomainBooking.BookingStatus.CANCELLED ? (byte)1 : (byte)0,
            CreatedUtc = model.CreatedUtc
        };
    }
}
