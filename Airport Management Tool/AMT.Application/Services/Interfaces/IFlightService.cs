using System;
using AMT.Application.Dtos;
using AMT.Domain.Models;
using AMT.Domain.Utils;

namespace AMT.Application.Services.Interfaces;

public interface IFlightService
{
    Task<Result<Flight, Exception>> CreateFlightAsync(FlightRequest flightRequest);
    Task<Result<List<Flight>, Exception>> FilterFlightsBy(string? date = null, string? origin = null, string? destination = null);
    Task<Result<Flight, Exception>> UpdateFlightAsync(FlightRequest flightRequest);
}
