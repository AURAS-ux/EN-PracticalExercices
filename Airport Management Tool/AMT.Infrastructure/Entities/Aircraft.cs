using System;
using System.Collections.Generic;

namespace AMT.Infrastructure.Entities;

public partial class Aircraft
{
    public int Id { get; set; }

    public string TailNumber { get; set; } = null!;

    public string Model { get; set; } = null!;

    public int SeatCapacity { get; set; }

    public int OwnedByAirlineId { get; set; }

    public virtual ICollection<FlightSchedule> FlightSchedules { get; set; } = new List<FlightSchedule>();

    public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();

    public virtual Airline OwnedByAirline { get; set; } = null!;
}
