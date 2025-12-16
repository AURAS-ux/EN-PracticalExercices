using DomainAircraft = AMT.Domain.Models.Aircraft;
using DomainAirline = AMT.Domain.Models.Airline;
using DomainAirport = AMT.Domain.Models.Airport;
using DomainFlight = AMT.Domain.Models.Flight;
using DomainFlightSchedule = AMT.Domain.Models.FlightSchedule;
using DomainGate = AMT.Domain.Models.Gate;
using EntityFlightSchedule = AMT.Infrastructure.Entities.FlightSchedule;

namespace AMT.Infrastructure.Mappers;

public static class FlightScheduleMap
{
    public static DomainFlightSchedule ToDomain(EntityFlightSchedule entity)
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

        var gate = entity.Gate != null
            ? GateMap.ToDomain(entity.Gate)
            : new DomainGate { Id = entity.GateId ?? 0, Code = string.Empty, Airport = new DomainAirport { Id = 0, IATACode = string.Empty, Name = string.Empty, City = string.Empty, Country = string.Empty, Timezone = string.Empty } };

        var aircraft = entity.AssignedAircraft != null
            ? AircraftMap.ToDomain(entity.AssignedAircraft)
            : new DomainAircraft { Id = entity.AssignedAircraftId ?? 0, TailNumber = string.Empty, Model = string.Empty, SeatCapacity = 0, OwnedByAirline = new DomainAirline { Id = 0, Iatacode = string.Empty, Name = string.Empty } };

        var status = entity.Status switch
        {
            1 => DomainFlightSchedule.ScheduleStatus.BOARDING,
            2 => DomainFlightSchedule.ScheduleStatus.DEPARTED,
            3 => DomainFlightSchedule.ScheduleStatus.CANCELLED,
            4 => DomainFlightSchedule.ScheduleStatus.DELAYED,
            _ => DomainFlightSchedule.ScheduleStatus.PLANNED
        };

        return new DomainFlightSchedule
        {
            Id = entity.Id,
            Flight = flight,
            ScheduledDepartureUtc = entity.ScheduledDepartureUtc,
            ScheduledArrivalUtc = entity.ScheduledArrivalUtc,
            Gate = gate,
            AssignedAircraft = aircraft,
            Status = status
        };
    }

    public static EntityFlightSchedule ToEntity(DomainFlightSchedule model)
    {
        return new EntityFlightSchedule
        {
            Id = model.Id,
            FlightId = model.Flight.Id,
            ScheduledDepartureUtc = model.ScheduledDepartureUtc,
            ScheduledArrivalUtc = model.ScheduledArrivalUtc,
            GateId = model.Gate.Id == 0 ? null : model.Gate.Id,
            AssignedAircraftId = model.AssignedAircraft.Id == 0 ? null : model.AssignedAircraft.Id,
            Status = model.Status switch
            {
                DomainFlightSchedule.ScheduleStatus.BOARDING => (byte)1,
                DomainFlightSchedule.ScheduleStatus.DEPARTED => (byte)2,
                DomainFlightSchedule.ScheduleStatus.CANCELLED => (byte)3,
                DomainFlightSchedule.ScheduleStatus.DELAYED => (byte)4,
                _ => (byte)0
            }
        };
    }
}
