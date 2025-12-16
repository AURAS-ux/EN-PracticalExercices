using DomainAircraft = AMT.Domain.Models.Aircraft;
using DomainAirline = AMT.Domain.Models.Airline;
using DomainAirport = AMT.Domain.Models.Airport;
using DomainFlight = AMT.Domain.Models.Flight;
using EntityFlight = AMT.Infrastructure.Entities.Flight;

namespace AMT.Infrastructure.Mappers;

public static class FlightMap
{
    public static DomainFlight ToDomain(EntityFlight entity)
    {
        var airline = entity.Airline != null
            ? AirlineMap.ToDomain(entity.Airline)
            : new DomainAirline { Id = entity.AirlineId, IATACode = string.Empty, Name = string.Empty };

        var origin = entity.OriginAirport != null
            ? AirportMap.ToDomain(entity.OriginAirport)
            : new DomainAirport { Id = entity.OriginAirportId, IATACode = string.Empty, Name = string.Empty, City = string.Empty, Country = string.Empty, Timezone = string.Empty };

        var destination = entity.DestinationAirport != null
            ? AirportMap.ToDomain(entity.DestinationAirport)
            : new DomainAirport { Id = entity.DestinationAirportId, IATACode = string.Empty, Name = string.Empty, City = string.Empty, Country = string.Empty, Timezone = string.Empty };

        var defaultAircraft = entity.DefaultAircraft != null
            ? AircraftMap.ToDomain(entity.DefaultAircraft)
            : new DomainAircraft
            {
                Id = entity.DefaultAircraftId ?? 0,
                TailNumber = string.Empty,
                Model = string.Empty,
                SeatCapacity = 0,
                OwnedByAirline = airline
            };

        return new DomainFlight
        {
            Id = entity.Id,
            Airline = airline,
            FlightNumber = entity.FlightNumber,
            OriginAirport = origin,
            DestinationAirport = destination,
            DefaultAircraft = defaultAircraft,
            IsActive = entity.IsActive ?? true
        };
    }

    public static EntityFlight ToEntity(DomainFlight model)
    {
        return new EntityFlight
        {
            Id = model.Id,
            AirlineId = model.Airline.Id,
            FlightNumber = model.FlightNumber,
            OriginAirportId = model.OriginAirport.Id,
            DestinationAirportId = model.DestinationAirport.Id,
            DefaultAircraftId = model.DefaultAircraft.Id == 0 ? null : model.DefaultAircraft.Id,
            IsActive = model.IsActive
        };
    }
}
