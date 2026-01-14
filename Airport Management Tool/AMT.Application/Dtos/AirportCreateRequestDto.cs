using System.ComponentModel.DataAnnotations;

namespace AMT.Application.Dtos;

public record class AirportCreateRequestDto
{
    [StringLength(3, MinimumLength = 3, ErrorMessage = "IATA code must be exactly 3 characters.")]
    public required string IATACode { get; set; } 
    [StringLength(120, ErrorMessage = "Airport name cannot exceed 120 characters.")]
    public required string Name { get; set; } 
    [StringLength(80, ErrorMessage = "City name cannot exceed 80 characters.")]
    public required string City { get; set; } 
    [StringLength(80, ErrorMessage = "Country name cannot exceed 80 characters.")]
    public required string Country { get; set; } 
    public required string Timezone { get; set; } 
}
