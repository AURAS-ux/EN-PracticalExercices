namespace AMT.Application.Dtos;

public record class UpcomingFlightsDto
{
    public string Date { get; set; }
    public int Flights { get; set; }
}
