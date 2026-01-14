using DomainAircraft = AMT.Domain.Models.Aircraft;
using DomainAirline = AMT.Domain.Models.Airline;
using EntityAircraft = AMT.Infrastructure.Entities.Aircraft;

namespace AMT.Infrastructure.Mappers;

public static class AircraftMap
{
    public static DomainAircraft ToDomain(EntityAircraft entity)
    {
        var airline = entity.OwnedByAirline != null
            ? AirlineMap.ToDomain(entity.OwnedByAirline)
            : new DomainAirline { Id = entity.OwnedByAirlineId, Iatacode = string.Empty, Name = string.Empty };

        return new DomainAircraft
        {
            Id = entity.Id,
            TailNumber = entity.TailNumber,
            Model = entity.Model,
            SeatCapacity = entity.SeatCapacity,
            OwnedByAirline = airline
        };
    }

    public static EntityAircraft ToEntity(DomainAircraft model)
    {
        return new EntityAircraft
        {
            Id = model.Id,
            TailNumber = model.TailNumber,
            Model = model.Model,
            SeatCapacity = model.SeatCapacity,
            OwnedByAirlineId = model.OwnedByAirline.Id
        };
    }
}
