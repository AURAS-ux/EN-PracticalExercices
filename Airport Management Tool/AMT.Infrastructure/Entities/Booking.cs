using System;
using System.Collections.Generic;

namespace AMT.Infrastructure.Entities;

public partial class Booking
{
    public int Id { get; set; }

    public int FlightId { get; set; }

    public int TicketId { get; set; }

    public string PassagerFullName { get; set; } = null!;

    public string PassagerEmail { get; set; } = null!;

    public string ConfirmationCode { get; set; } = null!;

    public int Quantity { get; set; }

    public byte Status { get; set; }

    public DateTime? CreatedUtc { get; set; }

    public virtual Flight Flight { get; set; } = null!;

    public virtual Ticket Ticket { get; set; } = null!;
}
