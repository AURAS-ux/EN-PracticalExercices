using DomainAirport = AMT.Domain.Models.Airport;
using EntityAirport = AMT.Infrastructure.Entities.Airport;

namespace AMT.Infrastructure.Mappers;

public static class AirportMap
{
    public static DomainAirport ToDomain(EntityAirport entity)
    {
        return new DomainAirport
        {
            Id = entity.Id,
            IATACode = entity.Iatacode,
            Name = entity.Name ?? string.Empty,
            City = entity.City ?? string.Empty,
            Country = entity.Country ?? string.Empty,
            Timezone = entity.Timezone ?? string.Empty
        };
    }

    public static EntityAirport ToEntity(DomainAirport model)
    {
        return new EntityAirport
        {
            Id = model.Id,
            Iatacode = model.IATACode,
            Name = model.Name,
            City = model.City,
            Country = model.Country,
            Timezone = model.Timezone
        };
    }
}
