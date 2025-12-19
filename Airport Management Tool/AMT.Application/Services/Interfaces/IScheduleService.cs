using System;
using AMT.Application.Dtos;
using AMT.Domain.Models;
using AMT.Domain.Utils;

namespace AMT.Application.Services.Interfaces;

public interface IScheduleService
{
    Result<FlightSchedule,Exception> GetSchedule(int id);
    Result<List<UpcomingFlightsDto>,Exception> GetUpcomingSchedules(string date);
    Task<Result<FlightSchedule,Exception>> CreateScheduleAsync(CreateScheduleDto scheduleRequest);
    Task<BulkImportResultDto> BulkCreateSchedulesAsync(string rawData);
}
