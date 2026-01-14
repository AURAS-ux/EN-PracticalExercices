using System;
using System.Collections.Generic;

namespace AMT.Infrastructure.Entities;

public partial class Ticket
{
    public int Id { get; set; }

    public int FlightId { get; set; }

    public string FareClass { get; set; } = null!;

    public decimal BasePrice { get; set; }

    public decimal Taxes { get; set; }

    public decimal? TotalPrice { get; set; }

    public string Currency { get; set; } = null!;

    public bool? IsRefundable { get; set; }

    public int SeatInventory { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Flight Flight { get; set; } = null!;
}
