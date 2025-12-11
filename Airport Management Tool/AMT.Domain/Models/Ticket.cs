namespace AMT.Domain.Models;

public class Ticket
{
    public int Id { get; set; }
    public Flight Flight { get; set; } = null!;
    public string FareClass { get; set; } = null!;
    public decimal BasePrice { get; set; }
    public string Currency { get; set; } = null!;
    public bool IsRefundable { get; set; } = false;
    public int SeatInventory { get; set; }
}
