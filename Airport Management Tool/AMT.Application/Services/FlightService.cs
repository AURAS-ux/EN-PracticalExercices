using System.Data;
using System.Net;
using System.Text.Json;
using AMT.Application.Dtos;
using AMT.Application.Dtos.Extentions;
using AMT.Application.Services.Interfaces;
using AMT.Domain.Models;
using AMT.Domain.Utils;
using AMT.Infrastructure.Exceptions;
using AMT.Infrastructure.Interfaces;
using FluentValidation;
using Serilog;

namespace AMT.Application.Services;

public class FlightService(IUnitOfWork unitOfWork,IValidator<Flight> validator,ILogger logger) : IFlightService
{
    public async Task<Result<Flight, Exception>> CreateFlightAsync(CreateFlightDto flightRequest)
    {
        logger.Information("Creating flight");
        var flightResult =  await this.GetFlightEntity(flightRequest);
        if (flightResult.IsSuccess)
        {
            if (unitOfWork.Flights.ActiveFlightExists(flightRequest.FlightNumber))
            {
                logger.Warning("Flight with the same flight number already exists");
                return Result<Flight, Exception>
                .Failure(new List<string> { "Flight with the same flight number already exists." }, 
                new List<Exception> { new DuplicateNameException("Flight with the same flight number already exists.") }, 
                HttpStatusCode.Conflict);
            }
            await unitOfWork.Flights.AddAsync(flightResult.Value!);
            await unitOfWork.SaveChangesAsync();
            return Result<Flight, Exception>.Success(flightResult.Value!);
        }
        logger.Warning("Found errors trying to save flight");
        return flightResult;
    }

    public async Task<Result<string, Exception>> DeleteFlightAsync(int flightId)
    {
        try
        {
            unitOfWork.Flights.Delete(flightId);
            await unitOfWork.SaveChangesAsync();
            return Result<string, Exception>
            .Success($"Flight with ID {flightId} deleted successfully.");
        }
        catch (GenericNotFound<Flight, int> ex)
        {
            return Result<string, Exception>.Failure(
                new List<string> { ex.Message },
                new List<Exception> { ex },
                HttpStatusCode.NotFound);
        }
    }

    public async Task<Result<Flight, Exception>> UpdateFlightAsync(int id, UpdateFlightDto flightRequest)
    {
        var flight = unitOfWork.Flights.GetById(id);
        if (flight == null)
        {
            return Result<Flight, Exception>
            .Failure(new List<string> { "Flight not found." }, 
            [new GenericNotFound<Flight,int>(id)], 
            HttpStatusCode.NotFound);
        }
        if(flightRequest.FlightNumber != null)
        {
            flight.FlightNumber = flightRequest.FlightNumber;
        }
        if(flightRequest.AirlineIata != null)
        {
            if(unitOfWork.Airlines.GetByIataCode(flightRequest.AirlineIata!) == null)
            {
                return Result<Flight, Exception>
                .Failure(new List<string> { "Airline not found." }, 
                [new GenericNotFound<Airline,string>(flightRequest.AirlineIata!)], 
                HttpStatusCode.NotFound);
            }
            flight.Airline = unitOfWork.Airlines.GetByIataCode(flightRequest.AirlineIata!)!;
        }
        if(flightRequest.OriginIata != null)
        {
            if(unitOfWork.Airports.GetByIataCode(flightRequest.OriginIata!) == null)
            {
                return Result<Flight, Exception>
                .Failure(new List<string> { "Origin Airport not found." }, 
                [new GenericNotFound<Airport,string>(flightRequest.OriginIata!)], 
                HttpStatusCode.NotFound);
            }
            flight.OriginAirport = unitOfWork.Airports.GetByIataCode(flightRequest.OriginIata!)!;
        }
        if(flightRequest.DestinationIata != null)
        {
            if(unitOfWork.Airports.GetByIataCode(flightRequest.DestinationIata!) == null)
            {
                return Result<Flight, Exception>
                .Failure(new List<string> { "Destination Airport not found." }, 
                [new GenericNotFound<Airport,string>(flightRequest.DestinationIata!)], 
                HttpStatusCode.NotFound);
            }
            flight.DestinationAirport = unitOfWork.Airports.GetByIataCode(flightRequest.DestinationIata!)!;
        }
        if(flightRequest.DefaultAircraftTail != null)
        {
            var aircraft = unitOfWork.Aircraft.GetByTailNumber(flightRequest.DefaultAircraftTail);
            if (aircraft == null)
            {
                return Result<Flight, Exception>.Failure(
                    new List<string> { $"Aircraft with Tail Number {flightRequest.DefaultAircraftTail} not found." },
                    new List<Exception> { new KeyNotFoundException() },
                    HttpStatusCode.NotFound);
            }
            flight.DefaultAircraft = aircraft;
        }
        if(flightRequest.IsActive != null)
        {
            flight.IsActive = flightRequest.IsActive.Value;
        }

        var validationResult = validator.Validate(flight);
        if(!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return Result<Flight, Exception>.Failure(errors, 
            [new Infrastructure.Exceptions.ValidationException(JsonSerializer.Serialize(errors))], 
            HttpStatusCode.BadRequest);
        }

        unitOfWork.Flights.Update(flight);
        await unitOfWork.SaveChangesAsync();

        return Result<Flight, Exception>.Success(flight);
    }

    private async Task<Result<Flight, Exception>> GetFlightEntity(CreateFlightDto flightRequest)
    {
        var airline = unitOfWork.Airlines.GetByIataCode(flightRequest.AirlineIata);
        var originAirport = unitOfWork.Airports.GetByIataCode(flightRequest.OriginIata);
        var destinationAirport = unitOfWork.Airports.GetByIataCode(flightRequest.DestinationIata);
        Aircraft? aircraft = null;
        if(airline == null)
        {
            return Result<Flight, Exception>
            .Failure(new List<string> { "Airline not found." }, 
            [new GenericNotFound<Airline,string>(flightRequest.AirlineIata)], 
            System.Net.HttpStatusCode.NotFound);
        }
        if(originAirport == null)
        {
            return Result<Flight, Exception>
            .Failure(new List<string> { "Origin Airport not found." }, 
            [new GenericNotFound<Airport,string>(flightRequest.OriginIata)], 
            System.Net.HttpStatusCode.NotFound);
        }
        if(destinationAirport == null)
        {
            return Result<Flight, Exception>
            .Failure(new List<string> { "Destination Airport not found." }, 
            [new GenericNotFound<Airport,string>(flightRequest.DestinationIata)], 
            System.Net.HttpStatusCode.NotFound);
        }
        if (!string.IsNullOrEmpty(flightRequest.DefaultAircraftTail))
        {
            aircraft = unitOfWork.Aircraft.GetByTailNumber(flightRequest.DefaultAircraftTail);
            if (aircraft == null)
            {
                return Result<Flight, Exception>.Failure(
                    new List<string> { $"Aircraft with Tail Number {flightRequest.DefaultAircraftTail} not found." },
                    new List<Exception> { new KeyNotFoundException() },
                    HttpStatusCode.NotFound);
            } 
        }

        var flight = flightRequest.ToFlight(airline, originAirport, destinationAirport, aircraft); 

        var validationResult = validator.Validate(flight);
        if(!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return Result<Flight, Exception>.Failure(errors, 
            [new Infrastructure.Exceptions.ValidationException(JsonSerializer.Serialize(errors))], 
            HttpStatusCode.BadRequest);
        }
        return Result<Flight, Exception>.Success(flight);
    }
}
