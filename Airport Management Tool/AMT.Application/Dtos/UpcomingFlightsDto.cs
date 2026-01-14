namespace AMT.Application.Dtos;

public record class UpcomingFlightsDto
{
    public string Date { get; set; } = null!;
    public int Flights { get; set; }
}
