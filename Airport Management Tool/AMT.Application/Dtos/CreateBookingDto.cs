namespace AMT.Application.Dtos;

public record class CreateBookingDto
{
    public int FlightScheduleId { get; set; }
    public int TicketId { get; set; }
    public required string PassengerFullName { get; set; }
    public required string PassengerEmail { get; set; }
    public int Quantity { get; set; }
}
