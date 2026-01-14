using System;
using AMT.Application.Dtos;
using AMT.Domain.Models;
using AMT.Domain.Utils;

namespace AMT.Application.Services.Interfaces;

public interface IAircraftService
{
    Task<Result<Aircraft,Exception>> CreateAircraftAsync(AircraftCreateRequestDto aircraftCreateRequestDto);
    Result<Aircraft,Exception> GetAircraftByIdAsync(int id);
    Result<IEnumerable<Aircraft>,Exception> GetAllAircrafts();
}
