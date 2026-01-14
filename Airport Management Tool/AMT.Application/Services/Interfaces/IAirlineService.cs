using System;
using AMT.Application.Dtos;
using AMT.Domain.Models;
using AMT.Domain.Utils;

namespace AMT.Application.Services.Interfaces;

public interface IAirlineService
{
    Result<Airline, Exception> GetAirlineByIdAsync(int id);
    Task<Result<Airline, Exception>> CreateAirlineAsync(AirlineCreateRequestDto airlineCreateRequestDto);
    Result<IEnumerable<Airline>, Exception> GetAllAirlinesAsync();
}
