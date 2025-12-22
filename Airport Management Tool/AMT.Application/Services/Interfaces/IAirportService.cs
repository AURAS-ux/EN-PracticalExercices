using System;
using AMT.Application.Dtos;
using AMT.Domain.Models;
using AMT.Domain.Utils;

namespace AMT.Application.Services.Interfaces;

public interface IAirportService
{
    Task<Result<Airport,Exception>> CreateAirportAsync(AirportCreateRequestDto airportCreateRequestDto);
    Task<Result<Airport,Exception>> GetAirportById(int id);
    Result<IEnumerable<Airport>,Exception> GetAllAirports();
}
