using DomainAirline = AMT.Domain.Models.Airline;
using EntityAirline = AMT.Infrastructure.Entities.Airline;

namespace AMT.Infrastructure.Mappers;

public static class AirlineMap
{
    public static DomainAirline ToDomain(EntityAirline entity)
    {
        return new DomainAirline
        {
            Id = entity.Id,
            Iatacode = entity.Iatacode,
            Name = entity.Name
        };
    }

    public static EntityAirline ToEntity(DomainAirline model)
    {
        return new EntityAirline
        {
            Id = model.Id,
            Iatacode = model.Iatacode,
            Name = model.Name
        };
    }
}
