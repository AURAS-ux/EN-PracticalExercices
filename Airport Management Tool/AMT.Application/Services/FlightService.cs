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

namespace AMT.Application.Services;

public class FlightService(IUnitOfWork unitOfWork,IValidator<Flight> validator) : IFlightService
{
    public async Task<Result<Flight, Exception>> CreateFlightAsync(FlightRequest flightRequest)
    {
        var airline = unitOfWork.Airlines.GetById(flightRequest.AirlineId);
        var originAirport = unitOfWork.Airports.GetById(flightRequest.OriginAirportId);
        var destinationAirport = unitOfWork.Airports.GetById(flightRequest.DestinationAirportId);
        Aircraft? aircraft = null;
        if(airline == null)
        {
            return Result<Flight, Exception>
            .Failure(new List<string> { "Airline not found." }, 
            [new GenericNotFound<Airline,int>(flightRequest.AirlineId)], 
            System.Net.HttpStatusCode.NotFound);
        }
        if(originAirport == null)
        {
            return Result<Flight, Exception>
            .Failure(new List<string> { "Origin Airport not found." }, 
            [new GenericNotFound<Airport,int>(flightRequest.OriginAirportId)], 
            System.Net.HttpStatusCode.NotFound);
        }
        if(destinationAirport == null)
        {
            return Result<Flight, Exception>
            .Failure(new List<string> { "Destination Airport not found." }, 
            [new GenericNotFound<Airport,int>(flightRequest.DestinationAirportId)], 
            System.Net.HttpStatusCode.NotFound);
        }
        if (flightRequest.DefaultAircraftId.HasValue)
        {
            aircraft = unitOfWork.Aircraft.GetById(flightRequest.DefaultAircraftId.Value);
            if (aircraft == null)
            {
                return Result<Flight, Exception>.Failure(
                    new List<string> { $"Aircraft with ID {flightRequest.DefaultAircraftId} not found." },
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
        await unitOfWork.Flights.AddAsync(flight);
        await unitOfWork.SaveChangesAsync();

        System.Console.WriteLine("Requesting flight after save:");
        var addedFlight = unitOfWork.Flights.GetById(flight.Id);

        return Result<Flight, Exception>.Success(flight);
    }

    public Task<Result<List<Flight>, Exception>> FilterFlightsBy(string? date = null, string? origin = null, string? destination = null)
    {
        //TODO implement flightschedule and process dates there
        // var flights = unitOfWork.Flights.GetAll().Select(flight =>
        // {
        //     if (date != null && flight. != DateTime.Parse(date).Date)
        //     {
        //         return false;
        //     }
        //     if (origin != null && !flight.OriginAirport.Code.Equals(origin, StringComparison.OrdinalIgnoreCase))
        //     {
        //         return false;
        //     }
        //     if (destination != null && !flight.DestinationAirport.Code.Equals(destination, StringComparison.OrdinalIgnoreCase))
        //     {
        //         return false;
        //     }
        //     return true;
        // }).ToList();
        throw new NotImplementedException();
    }

    public Task<Result<Flight, Exception>> UpdateFlightAsync(FlightRequest flightRequest)
    {
        throw new NotImplementedException();
    }
}
