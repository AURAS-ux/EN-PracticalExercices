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
        this CreateFlightDto dto,
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
    public static CreateFlightDto ToCreateFlightDto(this Flight flight)
    {
        return new CreateFlightDto
        {
            AirlineIata = flight.Airline.Iatacode,
            FlightNumber = flight.FlightNumber,
            OriginIata = flight.OriginAirport.IATACode,
            DestinationIata = flight.DestinationAirport.IATACode,
            DefaultAircraftTail = flight.DefaultAircraft?.TailNumber!,
            IsActive = flight.IsActive
        };
    }
}