using System;
using AMT.Application.Dtos;
using AMT.Domain.Models;
using AMT.Domain.Utils;

namespace AMT.Application.Services.Interfaces;

public interface IFlightService
{
    Task<Result<Flight, Exception>> CreateFlightAsync(CreateFlightDto flightRequest);
    Task<Result<Flight, Exception>> UpdateFlightAsync(int id, UpdateFlightDto flightRequest);
    Task<Result<string, Exception>> DeleteFlightAsync(int flightId);
    Result<IEnumerable<Flight>, Exception> GetAllFlights();
}
