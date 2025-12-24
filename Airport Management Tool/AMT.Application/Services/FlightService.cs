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
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AMT.Application.Services;

public class FlightService(IUnitOfWork unitOfWork,IValidator<Flight> validator,ILogger logger) : IFlightService
{
    public async Task<Result<Flight, Exception>> CreateFlightAsync(CreateFlightDto flightRequest)
    {
        logger.Information("Creating flight");
        var flightResult =  this.GetFlightEntity(flightRequest);
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
            var savedFlight = unitOfWork.Flights.GetFlightByFlightNumber(flightRequest.FlightNumber);
            return Result<Flight, Exception>.Success(savedFlight!);
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
        }catch (DbUpdateException ex)
        {
            logger.Error("Cannot delete flight as it is referenced by other records.", ex);
            var scheduleId = unitOfWork.FlightSchedules.GetScheduleIdByFlightId(flightId);
            var ticketId = unitOfWork.Tickets.GetTicketIdByFlightId(flightId);
            var confirmationCode = unitOfWork.Bookings.GetConfirmationCodeForFlightId(flightId);
            List<string> errorMessages = new List<string>();
            if (scheduleId != null)
            {
                errorMessages.Add("Flight is associated with a flight schedule.");
            }
            if (ticketId != null)
            {
                errorMessages.Add("Flight is associated with a ticket.");
            }
            if (confirmationCode != null)
            {
                errorMessages.Add("Flight is associated with a booking.");
            }
            errorMessages.Add("Cannot delete flight as it is referenced by other records.");
            logger.Error(ex.StackTrace ?? string.Empty);
            return Result<string, Exception>.Failure(
                errorMessages,
                new List<Exception> { ex },
                HttpStatusCode.InternalServerError);
        }
    }

    public Result<IEnumerable<Flight>, Exception> GetAllFlights()
    {
        var flights = unitOfWork.Flights.GetAll();
        return Result<IEnumerable<Flight>, Exception>.Success(flights);
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

    private Result<Flight, Exception> GetFlightEntity(CreateFlightDto flightRequest)
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
            HttpStatusCode.NotFound);
        }
        if(originAirport == null)
        {
            return Result<Flight, Exception>
            .Failure(new List<string> { "Origin Airport not found." }, 
            [new GenericNotFound<Airport,string>(flightRequest.OriginIata)],
            HttpStatusCode.NotFound);
        }
        if(destinationAirport == null)
        {
            return Result<Flight, Exception>
            .Failure(new List<string> { "Destination Airport not found." }, 
            [new GenericNotFound<Airport,string>(flightRequest.DestinationIata)],
            HttpStatusCode.NotFound);
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
