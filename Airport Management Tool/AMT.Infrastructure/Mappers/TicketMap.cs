using DomainAircraft = AMT.Domain.Models.Aircraft;
using DomainAirline = AMT.Domain.Models.Airline;
using DomainAirport = AMT.Domain.Models.Airport;
using DomainFlight = AMT.Domain.Models.Flight;
using DomainTicket = AMT.Domain.Models.Ticket;
using EntityTicket = AMT.Infrastructure.Entities.Ticket;

namespace AMT.Infrastructure.Mappers;

public static class TicketMap
{
    public static DomainTicket ToDomain(EntityTicket entity)
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

        return new DomainTicket
        {
            Id = entity.Id,
            Flight = flight,
            FareClass = entity.FareClass,
            BasePrice = entity.BasePrice,
            Taxes = entity.Taxes,
            Currency = entity.Currency,
            IsRefundable = entity.IsRefundable ?? false,
            SeatInventory = entity.SeatInventory
        };
    }

    public static EntityTicket ToEntity(DomainTicket model)
    {
        return new EntityTicket
        {
            Id = model.Id,
            FlightId = model.Flight.Id,
            FareClass = model.FareClass,
            BasePrice = model.BasePrice,
            Taxes = model.Taxes,
            Currency = model.Currency,
            IsRefundable = model.IsRefundable,
            SeatInventory = model.SeatInventory
        };
    }
}
