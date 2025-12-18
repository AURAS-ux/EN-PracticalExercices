using System.ComponentModel.DataAnnotations;

namespace AMT.Domain.Models;

public class Aircraft
{
    public int Id { get; set; }
    [StringLength(10, ErrorMessage = "Tail number cannot exceed 10 characters.")]
    public string TailNumber { get; set; } = null!;
    [StringLength(60, ErrorMessage = "Model name cannot exceed 60 characters.")]
    public string Model { get; set; } = null!;
    [Range(0, int.MaxValue, ErrorMessage = "Seat capacity must be a non-negative integer.")]
    public int SeatCapacity { get; set; }
    public Airline OwnedByAirline { get; set; } = null!;
}
