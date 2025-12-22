using System.ComponentModel.DataAnnotations;

namespace AMT.Application.Dtos;

public record class AircraftCreateRequestDto
{
    [StringLength(10, ErrorMessage = "Tail number cannot exceed 10 characters.")]
    public required string TailNumber { get; set; }
    [StringLength(60, ErrorMessage = "Model name cannot exceed 60 characters.")]
    public required string Model { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Seat capacity must be a non-negative integer.")]
    public required int SeatCapacity { get; set; }
    public required int OwnedByAirlineId { get; set; }
}
