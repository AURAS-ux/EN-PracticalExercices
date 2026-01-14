using System;
using AMT.Domain.Models;

namespace AMT.Infrastructure.Interfaces.Repos;

public interface IAirlineRepository : IGenericRepository<Airline>
{
    Airline? GetByIataCode(string iataCode);
}
