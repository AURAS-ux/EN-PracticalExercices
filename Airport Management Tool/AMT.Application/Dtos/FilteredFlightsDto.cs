namespace AMT.Application.Dtos;

public record class FilteredFlightsDto
{
    public string? FlightNumber { get; set; }
    public string? Origin { get; set; }
    public string? Destination { get; set; }
    public DateTime? DepartureTime { get; set; }
    public DateTime? ArrivalTime { get; set; }
}
