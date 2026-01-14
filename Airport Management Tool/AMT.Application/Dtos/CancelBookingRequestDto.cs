namespace AMT.Application.Dtos;

public record class CancelBookingRequestDto
{
    public required int BookingId { get; set; }
    public required string ConfirmationCode { get; set; }
}
