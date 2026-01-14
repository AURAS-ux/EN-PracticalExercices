using DomainAirport = AMT.Domain.Models.Airport;
using DomainGate = AMT.Domain.Models.Gate;
using EntityGate = AMT.Infrastructure.Entities.Gate;

namespace AMT.Infrastructure.Mappers;

public static class GateMap
{
    public static DomainGate ToDomain(EntityGate entity)
    {
        var airport = entity.Airport != null
            ? AirportMap.ToDomain(entity.Airport)
            : new DomainAirport
            {
                Id = entity.AirportId,
                IATACode = string.Empty,
                Name = string.Empty,
                City = string.Empty,
                Country = string.Empty,
                Timezone = string.Empty
            };

        return new DomainGate
        {
            Id = entity.Id,
            Code = entity.Code,
            Airport = airport
        };
    }

    public static EntityGate ToEntity(DomainGate model)
    {
        return new EntityGate
        {
            Id = model.Id,
            Code = model.Code,
            AirportId = model.Airport.Id
        };
    }
}
