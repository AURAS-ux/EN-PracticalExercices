using System.ComponentModel.DataAnnotations;

namespace AMT.Application.Dtos;

public record CreateFlightDto
{
    [Required(ErrorMessage = "Airline IATA code is required.")]
    public required string AirlineIata { get; set; }

    [Required(ErrorMessage = "Flight number is required.")]
    [StringLength(8, ErrorMessage = "Flight number cannot exceed 8 characters.")]
    public string FlightNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Origin airport IATA code is required.")]
    public required string OriginIata { get; set; }

    [Required(ErrorMessage = "Destination airport IATA code is required.")]
    public required string DestinationIata { get; set; }

    [Required(ErrorMessage = "Default aircraft tail number is required.")]
    public required string DefaultAircraftTail { get; set; }

    public bool IsActive { get; set; } = true;
}