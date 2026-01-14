using System.ComponentModel.DataAnnotations;

namespace AMT.Domain.Models;

public class Flight
{
    public int Id { get; set; }
    public Airline Airline { get; set; } = null!;
    [StringLength(8, ErrorMessage = "Flight number cannot exceed 8 characters.")]
    public string FlightNumber { get; set; } = null!;
    public Airport OriginAirport { get; set; } = null!;
    public Airport DestinationAirport { get; set; } = null!;
    public Aircraft DefaultAircraft { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}
