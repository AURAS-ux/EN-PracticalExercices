namespace AMT.Application.Dtos;

public record class BookingCreatedDto
{
    public int BookingId { get; set; }
    public required string ConfirmationCode { get; set; }
}
