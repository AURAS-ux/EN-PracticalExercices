namespace AMT.Application.Dtos;

public record class CreateTicketDto
{
    public int FlightId { get; set; }
    public required string FareClass { get; set; }
    public decimal BasePrice { get; set; }
    public decimal Taxes { get; set; }
    public required string Currency { get; set; }
    public bool IsRefoundable { get; set; }
    public int SeatInventory { get; set; }
}
