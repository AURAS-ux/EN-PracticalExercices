using System;
using System.Net;
using AMT.Application.Dtos;
using AMT.Application.Dtos.Extentions;
using AMT.Application.Services.Interfaces;
using AMT.Domain.Models;
using AMT.Domain.Utils;
using AMT.Infrastructure.Exceptions;
using AMT.Infrastructure.Interfaces;
using AMT.Infrastructure.Interfaces.Repos;
using FluentValidation;
using Serilog;

namespace AMT.Application.Services;

public class ScheduleService(IUnitOfWork unitOfWork, ILogger logger, IValidator<FlightSchedule> validator) : IScheduleService
{
    public Task<Result<FlightSchedule, Exception>> BulkCreateSchedulesAsync(string rawData)
    {
        throw new NotImplementedException(); //TODO: implement bulk create
    }

    public async Task<Result<FlightSchedule, Exception>> CreateScheduleAsync(CreateScheduleDto scheduleRequest)
    {
        logger.Information("Schedule creation started.");
        var flightScheuleResult = this.GetSchedule(scheduleRequest);
        if (flightScheuleResult.IsSuccess)
        {
            await unitOfWork.FlightSchedules.AddAsync(flightScheuleResult.Value!);
            await unitOfWork.SaveChangesAsync();
            logger.Information("Schedule created successfully.");
            return Result<FlightSchedule, Exception>.Success(flightScheuleResult.Value!);
        }
        logger.Warning("Found errors trying to save schedule.");
        return flightScheuleResult;
    }

    public Result<FlightSchedule, Exception> GetSchedule(int id)
    {
        var schedule = unitOfWork.FlightSchedules.GetById(id);
        if (schedule == null)
        {
            return Result<FlightSchedule, Exception>
            .Failure(new List<string> { "Flight schedule not found." },
            [new GenericNotFound<FlightSchedule, int>(id)],
            HttpStatusCode.NotFound);
        }
        return Result<FlightSchedule, Exception>.Success(schedule);
    }

    public Result<List<UpcomingFlightsDto>, Exception> GetUpcomingSchedules(string date)
    {
        List<UpcomingFlightsDto> upcomingFlights = new();
        try
        {
            var parsedDate = DateTime.Parse(date);
            for(int i = 0; i < 7; i++)
            {
                var currentDate = parsedDate.AddDays(i);
                var flights = unitOfWork.FlightSchedules.GetFlightSchedulesCountForDate(currentDate);
                upcomingFlights.Add(new UpcomingFlightsDto
                {
                    Date = currentDate.Date.ToString("yyyy-MM-dd"),
                    Flights = flights
                });
            }
            return Result<List<UpcomingFlightsDto>, Exception>.Success(upcomingFlights);
        }
        catch (FormatException ex)
        {
            logger.Error("Error parsing date: {Message}", ex.Message);
            return Result<List<UpcomingFlightsDto>, Exception>
            .Failure(new List<string> { "Invalid date format." },
            [ex],
            HttpStatusCode.BadRequest);
        }
    }

    private Result<FlightSchedule, Exception> GetSchedule(CreateScheduleDto createScheduleDto)
    {
        var flight = unitOfWork.Flights.GetById(createScheduleDto.FlightId);
        if (flight == null)
        {
            return Result<FlightSchedule, Exception>
            .Failure(new List<string> { "Flight not found." },
            [new GenericNotFound<Flight, int>(createScheduleDto.FlightId)],
            HttpStatusCode.NotFound);
        }
        var gate = unitOfWork.Gates.GetByGateCode(createScheduleDto.GateCode);
        if (gate == null)
        {
            return Result<FlightSchedule, Exception>
            .Failure(new List<string> { "Gate not found." },
            [new GenericNotFound<Gate, string>(createScheduleDto.GateCode)],
            HttpStatusCode.NotFound);
        }
        var aircraft = unitOfWork.Aircraft.GetByTailNumber(createScheduleDto.AssignedAircraftTail);
        if (aircraft == null)
        {
            return Result<FlightSchedule, Exception>
            .Failure(new List<string> { "Aircraft not found." },
            [new GenericNotFound<Aircraft, string>(createScheduleDto.AssignedAircraftTail)],
            HttpStatusCode.NotFound);
        }
        try
        {
            var schedule = createScheduleDto.ToFlightSchedule(flight, gate, aircraft);
            var validationResult = validator.Validate(schedule);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<FlightSchedule, Exception>
                .Failure(errors,
                validationResult.Errors.Select(e => new Infrastructure.Exceptions.ValidationException(e.ErrorMessage)).ToList<Exception>(),
                HttpStatusCode.BadRequest);
            }
            return Result<FlightSchedule, Exception>.Success(schedule);
        }
        catch (Exception ex)
        {
            logger.Error("Error creating schedule: {Message}", ex.Message);
            return Result<FlightSchedule, Exception>
            .Failure(new List<string> { "Error creating schedule." },
            [ex],
            HttpStatusCode.InternalServerError);
        }
    }
}
