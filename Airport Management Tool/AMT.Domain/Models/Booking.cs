namespace AMT.Domain.Models;

public class Booking
{
    public int Id { get; set; }
    public Flight Flight { get; set; } = null!;
    public Ticket Ticket { get; set; } = null!;
    public string PassengerFullName { get; set; } = null!;
    public string PassengerEmail { get; set; } = null!;
    public string ConfirmationCode { get; set; } = null!;
    public int Quantity { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    public enum Status : byte
    {
        ACTIVE = 0,
        CANCELLED = 1
    }
}

