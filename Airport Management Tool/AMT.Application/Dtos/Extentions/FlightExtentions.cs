using System;
using AMT.Domain.Models;

namespace AMT.Application.Dtos.Extentions;

public static class FlightExtensions
{
    /// <summary>
    /// Maps CreateFlightDto to Flight domain model.
    /// Requires fetched related entities (Airline, Airports, Aircraft).
    /// </summary>
    public static Flight ToFlight(
        this FlightRequest dto,
        Airline airline,
        Airport originAirport,
        Airport destinationAirport,
        Aircraft? defaultAircraft = null)
    {
        return new Flight
        {
            Airline = airline,
            FlightNumber = dto.FlightNumber,
            OriginAirport = originAirport,
            DestinationAirport = destinationAirport,
            DefaultAircraft = defaultAircraft!,
            IsActive = dto.IsActive
        };
    }

    /// <summary>
    /// Maps Flight domain model back to CreateFlightDto.
    /// </summary>
    public static FlightRequest ToCreateFlightDto(this Flight flight)
    {
        return new FlightRequest
        {
            AirlineId = flight.Airline.Id,
            FlightNumber = flight.FlightNumber,
            OriginAirportId = flight.OriginAirport.Id,
            DestinationAirportId = flight.DestinationAirport.Id,
            DefaultAircraftId = flight.DefaultAircraft?.Id,
            IsActive = flight.IsActive
        };
    }
}