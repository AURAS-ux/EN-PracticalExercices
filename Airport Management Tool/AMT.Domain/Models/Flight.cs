namespace AMT.Domain.Models;

public class Flight
{
    public int Id { get; set; }
    public Airline Airline { get; set; } = null!;
    public string FlightNumber { get; set; } = null!;
    public Airport OriginAirport { get; set; } = null!;
    public Airport DestinationAirport { get; set; } = null!;
    public Aircraft DefaultAircraft { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}
