using System;
using System.Collections.Generic;

namespace AMT.Infrastructure.Entities;

public partial class Airport
{
    public int Id { get; set; }

    public string Iatacode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? City { get; set; }

    public string? Country { get; set; }

    public string? Timezone { get; set; }

    public virtual ICollection<Flight> FlightDestinationAirports { get; set; } = new List<Flight>();

    public virtual ICollection<Flight> FlightOriginAirports { get; set; } = new List<Flight>();

    public virtual ICollection<Gate> Gates { get; set; } = new List<Gate>();
}
