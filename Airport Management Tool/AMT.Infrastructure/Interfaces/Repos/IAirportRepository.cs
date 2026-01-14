using System;
using AMT.Domain.Models;

namespace AMT.Infrastructure.Interfaces.Repos;

public interface IAirportRepository : IGenericRepository<Airport>
{
    Airport? GetByIataCode(string iataCode);
}
