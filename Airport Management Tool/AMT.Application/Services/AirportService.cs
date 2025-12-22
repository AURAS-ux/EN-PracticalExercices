using System;
using AMT.Application.Services.Interfaces;
using AMT.Domain.Models;
using AMT.Domain.Utils;
using AMT.Infrastructure.Interfaces;
using Serilog;

namespace AMT.Application.Services;

public class AirportService(IUnitOfWork unitOfWork, ILogger logger) : IAirportService
{
    public Task<Result<Airport, Exception>> CreateAirportAsync(Airport airport)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Airport, Exception>> GetAirportById(int id)
    {
        throw new NotImplementedException();
    }

    public Result<IEnumerable<Airport>, Exception> GetAllAirports()
    {
        var airports = unitOfWork.Airports.GetAll();
        if(airports is null || !airports.Any())
        {
            logger.Warning("No airports found in the database.");
            return Result<IEnumerable<Airport>, Exception>.Failure(new List<string> { "No airports found." },
            new List<Exception> { new Exception("No airports available in the database.") },
            System.Net.HttpStatusCode.NotFound);
        }
        return Result<IEnumerable<Airport>, Exception>.Success(airports);
    }
}
