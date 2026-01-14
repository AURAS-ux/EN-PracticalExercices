using System;
using AMT.Application.Dtos;
using AMT.Application.Services.Interfaces;
using AMT.Domain.Models;
using AMT.Domain.Utils;
using AMT.Infrastructure.Exceptions;
using AMT.Infrastructure.Interfaces;
using Serilog;

namespace AMT.Application.Services;

public class AirportService(IUnitOfWork unitOfWork, ILogger logger) : IAirportService
{
    public async Task<Result<Airport, Exception>> CreateAirportAsync(AirportCreateRequestDto airportCreateRequestDto)
    {
        var airport = new Airport
        {
            IATACode = airportCreateRequestDto.IATACode,
            Name = airportCreateRequestDto.Name,
            City = airportCreateRequestDto.City,
            Country = airportCreateRequestDto.Country,
            Timezone = airportCreateRequestDto.Timezone
        };
        await unitOfWork.Airports.AddAsync(airport);
        await unitOfWork.SaveChangesAsync();
        return Result<Airport, Exception>.Success(airport);
    }

    public Result<Airport, Exception> GetAirportById(int id)
    {
        var airport = unitOfWork.Airports.GetById(id);
        if(airport is null)
        {
            logger.Warning("Airport with ID {AirportId} not found.", id);
            return Result<Airport, Exception>.Failure(new List<string> { "Airport not found." },
            new List<Exception> { new GenericNotFound<Airport,int>(id) },
            System.Net.HttpStatusCode.NotFound);
        }
        return Result<Airport, Exception>.Success(airport);
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
