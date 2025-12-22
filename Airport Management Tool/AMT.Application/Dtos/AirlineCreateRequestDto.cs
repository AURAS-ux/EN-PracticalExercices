using System.ComponentModel.DataAnnotations;

namespace AMT.Application.Dtos;

public record class AirlineCreateRequestDto
{
    [StringLength(2, MinimumLength = 2, ErrorMessage = "IATA code must be exactly 2 characters.")]
    public required string Iatacode { get; set; }
    [StringLength(100, ErrorMessage = "Airline name cannot exceed 100 characters.")]
    public required string Name { get; set; }
}
