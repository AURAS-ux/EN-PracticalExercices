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

public class ScheduleService(IUnitOfWork unitOfWork, ILogger logger, IValidator<FlightSchedule> validator) : IScheduleService
{
    public async Task<BulkImportResultDto> BulkCreateSchedulesAsync(Stream fileStream)
    {
        try
        {
            using var reader = new StreamReader(fileStream);
            var rawData = await reader.ReadToEndAsync();
            var importResults = new Dictionary<BulkImportResultDto.ImportStatus, Result<FlightSchedule, Exception>?>();
            var scheduleDtos = JsonSerializer.Deserialize<List<CreateScheduleDto>>(rawData
            , new JsonSerializerOptions{PropertyNameCaseInsensitive =  true});
            if (scheduleDtos == null || !scheduleDtos.Any())
            {
                logger.Warning("No schedules found in the provided data.");
                return new BulkImportResultDto
                {
                    ImportResults = importResults
                };
            }
            foreach (var scheduleDto in scheduleDtos)
            {
                var result = await this.CreateScheduleAsync(scheduleDto);
                if (result.IsSuccess)
                {
                    importResults[BulkImportResultDto.ImportStatus.SUCCESS] = result;
                }
                else
                {
                    importResults[BulkImportResultDto.ImportStatus.FAILED] = result;
                }
            }
            logger.Information("Bulk schedule import completed.");
            return new BulkImportResultDto
            {
                ImportResults = importResults
            };
        }
        catch (JsonException ex)
        {
            logger.Error("Error parsing bulk schedule data: {Message}", ex.Message);
            return new BulkImportResultDto
            {
                ImportResults = new Dictionary<BulkImportResultDto.ImportStatus, Result<FlightSchedule, Exception>?>
                {
                    { BulkImportResultDto.ImportStatus.FAILED, Result<FlightSchedule, Exception>.Failure(
                        new List<string> { "Invalid JSON format." },
                        [ex],
                        HttpStatusCode.BadRequest) }
                }
            };
        }catch(ArgumentNullException ex)
        {
            logger.Error("Error reading bulk schedule data: {Message}", ex.Message);
            return new BulkImportResultDto
            {
                ImportResults = new Dictionary<BulkImportResultDto.ImportStatus, Result<FlightSchedule, Exception>?>
                {
                    { BulkImportResultDto.ImportStatus.FAILED, Result<FlightSchedule, Exception>.Failure(
                        new List<string> { "Input data is null." },
                        [ex],
                        HttpStatusCode.BadRequest) }
                }
            };
        }catch(ArgumentException ex)
        {
            logger.Error("Error reading bulk schedule data: {Message}", ex.Message);
            return new BulkImportResultDto
            {
                ImportResults = new Dictionary<BulkImportResultDto.ImportStatus, Result<FlightSchedule, Exception>?>
                {
                    { BulkImportResultDto.ImportStatus.FAILED, Result<FlightSchedule, Exception>.Failure(
                        new List<string> { "Error reading input data." },
                        [ex],
                        HttpStatusCode.BadRequest) }
                }
            };
        }
    }

    public async Task<Result<FlightSchedule, Exception>> CreateScheduleAsync(CreateScheduleDto scheduleRequest)
    {
        logger.Information("Schedule creation started.");
        var flightScheuleResult = this.GetSchedule(scheduleRequest);
        if (flightScheuleResult.IsSuccess)
        {
            if(unitOfWork.FlightSchedules.IsScheduleConflictForGate(flightScheuleResult.Value!.Gate.Id, flightScheuleResult.Value.ScheduledDepartureUtc) ||
               unitOfWork.FlightSchedules.IsScheduleConflictForGate(flightScheuleResult.Value.Gate.Id, flightScheuleResult.Value.ScheduledArrivalUtc))
            {
                logger.Warning("Schedule conflict detected for gate {GateId} at time {DepartureTime} or {ArrivalTime}.",
                    flightScheuleResult.Value.Gate.Id,
                    flightScheuleResult.Value.ScheduledDepartureUtc,
                    flightScheuleResult.Value.ScheduledArrivalUtc);
                return Result<FlightSchedule, Exception>
                .Failure(new List<string> 
                { $"Schedule conflict detected for the specified gate at the given time {flightScheuleResult.Value.ScheduledDepartureUtc} or {flightScheuleResult.Value.ScheduledArrivalUtc}." },
                [new Infrastructure.Exceptions.ValidationException("Schedule conflict detected.")],
                HttpStatusCode.Conflict);
            }
            await unitOfWork.FlightSchedules.AddAsync(flightScheuleResult.Value!);
            await unitOfWork.SaveChangesAsync();
            logger.Information("Schedule created successfully.");
            return Result<FlightSchedule, Exception>.Success(flightScheuleResult.Value!);
        }
        logger.Warning("Found errors trying to save schedule.");
        return flightScheuleResult;
    }

    public Result<List<FilteredFlightsDto>, Exception> FilterFlights(string? origin, string? destination, string? date)
    {
        List<FilteredFlightsDto> filteredFlights = new();
        try
        {
            DateTime? parsedDate = null;
            if (!string.IsNullOrEmpty(date))
            {
                parsedDate = DateTime.Parse(date);
            }
            var flights = unitOfWork.FlightSchedules.FilterFlightSchedules(origin, destination, parsedDate);
            foreach (var flight in flights)
            {
                filteredFlights.Add(flight.ToFilteredFlightsDto());
            }
            return Result<List<FilteredFlightsDto>, Exception>.Success(filteredFlights);
        }
        catch (FormatException ex)
        {
            logger.Error("Error parsing date: {Message}", ex.Message);
            return Result<List<FilteredFlightsDto>, Exception>
            .Failure(new List<string> { "Invalid date format." },
            [ex],
            HttpStatusCode.BadRequest);
        }
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
