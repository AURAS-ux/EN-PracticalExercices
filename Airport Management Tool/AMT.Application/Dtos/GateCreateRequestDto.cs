using System.ComponentModel.DataAnnotations;

namespace AMT.Application.Dtos;

public record class GateCreateRequestDto
{
    [StringLength(10, ErrorMessage = "Gate code cannot exceed 10 characters.")]
    public required string Code { get; set; }
    public required int AirportId { get; set; }
}
