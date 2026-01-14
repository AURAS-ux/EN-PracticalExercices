using System;
using System.Net;
using AMT.Application.Dtos;
using AMT.Application.Services.Interfaces;
using AMT.Domain.Models;
using AMT.Domain.Utils;
using AMT.Infrastructure.Exceptions;
using AMT.Infrastructure.Interfaces;
using Serilog;

namespace AMT.Application.Services;

public class AircraftService(IUnitOfWork unitOfWork, ILogger logger) : IAircraftService
{
    public async Task<Result<Aircraft, Exception>> CreateAircraftAsync(AircraftCreateRequestDto aircraftCreateRequestDto)
    {
        var owningAirline = unitOfWork.Airlines.GetById(aircraftCreateRequestDto.OwnedByAirlineId);
        if (owningAirline is null)
        {
            var errorMessage = $"Airline with ID {aircraftCreateRequestDto.OwnedByAirlineId} not found.";
            logger.Error(errorMessage);
            return Result<Aircraft, Exception>.Failure(new List<string> { errorMessage },
            new List<Exception> { new GenericNotFound<Airline, int>(aircraftCreateRequestDto.OwnedByAirlineId) },
            HttpStatusCode.NotFound);
        }
        var aircraft = new Aircraft
        {
            Model = aircraftCreateRequestDto.Model,
            TailNumber = aircraftCreateRequestDto.TailNumber,
            SeatCapacity = aircraftCreateRequestDto.SeatCapacity,
            OwnedByAirline = owningAirline
        };
        await unitOfWork.Aircraft.AddAsync(aircraft);
        await unitOfWork.SaveChangesAsync();
        return Result<Aircraft, Exception>.Success(aircraft);
    }

    public Result<Aircraft, Exception> GetAircraftByIdAsync(int id)
    {
        var aircraft = unitOfWork.Aircraft.GetById(id);
        if (aircraft is null)
        {
            var errorMessage = $"Aircraft with ID {id} not found.";
            logger.Error(errorMessage);
            return Result<Aircraft, Exception>.Failure(new List<string> { errorMessage },
            new List<Exception> { new GenericNotFound<Aircraft, int>(id) },
            HttpStatusCode.NotFound);
        }
        return Result<Aircraft, Exception>.Success(aircraft);
    }

    public Result<IEnumerable<Aircraft>, Exception> GetAllAircrafts()
    {
        var aircrafts = unitOfWork.Aircraft.GetAll();
        return Result<IEnumerable<Aircraft>, Exception>.Success(aircrafts);
    }
}
