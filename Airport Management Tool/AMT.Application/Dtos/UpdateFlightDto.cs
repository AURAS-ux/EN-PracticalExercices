using System;

namespace AMT.Application.Dtos;

public class UpdateFlightDto
{
    public string? AirlineIata { get; set; }
    public string? FlightNumber { get; set; }
    public string? OriginIata { get; set; }
    public string? DestinationIata { get; set; }
    public string? DefaultAircraftTail { get; set; }
    public bool? IsActive { get; set; }
}
