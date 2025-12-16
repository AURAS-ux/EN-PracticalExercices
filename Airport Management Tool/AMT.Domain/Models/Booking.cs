using System.ComponentModel.DataAnnotations;

namespace AMT.Domain.Models;

public class Booking
{
    public int Id { get; set; }
    public Flight Flight { get; set; } = null!;
    public Ticket Ticket { get; set; } = null!;
    [StringLength(120, ErrorMessage = "Passenger full name cannot exceed 120 characters.")]
    public string PassengerFullName { get; set; } = null!;
    [StringLength(120, ErrorMessage = "Passenger email cannot exceed 120 characters.")]
    public string PassengerEmail { get; set; } = null!;
    [StringLength(8, MinimumLength = 8, ErrorMessage = "Confirmation code must be exactly 8 characters.")]
    public string ConfirmationCode { get; set; } = null!;
    [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative integer.")]
    public int Quantity { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    public enum Status : byte
    {
        ACTIVE = 0,
        CANCELLED = 1
    }
}

