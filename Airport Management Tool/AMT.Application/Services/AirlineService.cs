using System;
using System.Net;
using AMT.Application.Dtos;
using AMT.Application.Services.Interfaces;
using AMT.Domain.Models;
using AMT.Domain.Utils;
using AMT.Infrastructure.Exceptions;
using AMT.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace AMT.Application.Services;

public class AirlineService(IUnitOfWork unitOfWork, ILogger logger) : IAirlineService
{
    public async Task<Result<Airline, Exception>> CreateAirlineAsync(AirlineCreateRequestDto airlineCreateRequestDto)
    {
        var airline = new Airline
        {
            Name = airlineCreateRequestDto.Name,
            Iatacode = airlineCreateRequestDto.Iatacode,
        };
        await unitOfWork.Airlines.AddAsync(airline);
        await unitOfWork.SaveChangesAsync();
        logger.Information("Airline created with ID: {AirlineId}", airline.Id);
        return Result<Airline, Exception>.Success(airline);
    }

    public Result<Airline, Exception> GetAirlineByIdAsync(int id)
    {
        var airline = unitOfWork.Airlines.GetById(id);
        if (airline == null)
        {
            logger.Warning("Airline with ID: {AirlineId} not found", id);
            return Result<Airline, Exception>.Failure(new List<string> { "Airline not found." },
            new List<Exception> { new GenericNotFound<Airline,int>(id) },
            HttpStatusCode.NotFound);
        }
        logger.Information("Airline with ID: {AirlineId} retrieved", id);
        return Result<Airline, Exception>.Success(airline);
    }

    public  Result<IEnumerable<Airline>, Exception> GetAllAirlinesAsync()
    {
        var airlines = unitOfWork.Airlines.GetAll();
        logger.Information("Retrieved all airlines, count: {AirlineCount}", airlines.Count());
        return Result<IEnumerable<Airline>, Exception>.Success(airlines);
    }
}
