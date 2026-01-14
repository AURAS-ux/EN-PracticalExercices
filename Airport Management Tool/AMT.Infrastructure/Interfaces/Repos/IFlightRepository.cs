using System;
using AMT.Domain.Models;

namespace AMT.Infrastructure.Interfaces.Repos;

public interface IFlightRepository : IGenericRepository<Flight>
{
    Flight? GetFlightByFlightNumber(string flightNumber);
    bool ActiveFlightExists(string flightNumber);
}
