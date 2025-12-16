using System.ComponentModel.DataAnnotations;

namespace AMT.Domain.Models;

public class Ticket
{
    public int Id { get; set; }
    public Flight Flight { get; set; } = null!;
    [StringLength(2, MinimumLength = 2, ErrorMessage = "Fare class must be exactly 2 characters.")]
    public string FareClass { get; set; } = null!;
    [Range(0, double.MaxValue, ErrorMessage = "Base price must be a non-negative value.")]
    public decimal BasePrice { get; set; }
    [Range(0, double.MaxValue, ErrorMessage = "Taxes must be a non-negative value.")]
    public decimal Taxes { get; set; }
    [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency code must be exactly 3 characters.")]
    public string Currency { get; set; } = null!;
    public bool IsRefundable { get; set; } = false;
    [Range(0, int.MaxValue, ErrorMessage = "Seat inventory must be a non-negative integer.")]
    public int SeatInventory { get; set; }
    public virtual IList<Booking> Bookings { get; set; } = null!;

}
