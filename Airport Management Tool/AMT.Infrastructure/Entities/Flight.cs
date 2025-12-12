using System;
using System.Collections.Generic;

namespace AMT.Infrastructure.Entities;

public partial class Flight
{
    public int Id { get; set; }

    public int AirlineId { get; set; }

    public string FlightNumber { get; set; } = null!;

    public int OriginAirportId { get; set; }

    public int DestinationAirportId { get; set; }

    public int? DefaultAircraftId { get; set; }

    public bool? IsActive { get; set; }

    public virtual Airline Airline { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Aircraft? DefaultAircraft { get; set; }

    public virtual Airport DestinationAirport { get; set; } = null!;

    public virtual ICollection<FlightSchedule> FlightSchedules { get; set; } = new List<FlightSchedule>();

    public virtual Airport OriginAirport { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
