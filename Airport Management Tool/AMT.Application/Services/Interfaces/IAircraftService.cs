using System;
using AMT.Domain.Models;
using AMT.Domain.Utils;

namespace AMT.Application.Services.Interfaces;

public interface IAircraftService
{
    Task<Result<Aircraft,Exception>> CreateAircraftAsync(Aircraft aircraft);
    Task<Result<Aircraft,Exception>> GetAircraftByIdAsync(int id);
}
