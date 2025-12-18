using System;
using AMT.Domain.Models;
using AMT.Domain.Utils;

namespace AMT.Application.Services.Interfaces;

public interface IAirportService
{
    Task<Result<Airport,Exception>> CreateAirportAsync(Airport airport);
    Task<Result<Airport,Exception>> GetAirportById(int id);
}
