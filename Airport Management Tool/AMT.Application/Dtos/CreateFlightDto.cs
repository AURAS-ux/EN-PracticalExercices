using System.ComponentModel.DataAnnotations;

namespace AMT.Application.Dtos;

public record FlightRequest
{
    [Required(ErrorMessage = "Airline ID is required.")]
    public int AirlineId { get; set; }

    [Required(ErrorMessage = "Flight number is required.")]
    [StringLength(8, ErrorMessage = "Flight number cannot exceed 8 characters.")]
    public string FlightNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Origin airport ID is required.")]
    public int OriginAirportId { get; set; }

    [Required(ErrorMessage = "Destination airport ID is required.")]
    public int DestinationAirportId { get; set; }

    public int? DefaultAircraftId { get; set; }

    public bool IsActive { get; set; } = true;
}