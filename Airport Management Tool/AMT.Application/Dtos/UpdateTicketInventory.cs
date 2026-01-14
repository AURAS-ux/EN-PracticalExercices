namespace AMT.Application.Dtos;

public record class UpdateTicketInventoryDto
{
    public int TicketId { get; set; }
    public int NewSeatInventory { get; set; }
}
