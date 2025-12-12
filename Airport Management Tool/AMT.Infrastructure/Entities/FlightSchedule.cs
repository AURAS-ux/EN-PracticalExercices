using System;
using System.Collections.Generic;

namespace AMT.Infrastructure.Entities;

public partial class FlightSchedule
{
    public int Id { get; set; }

    public int FlightId { get; set; }

    public DateTime ScheduledDepartureUtc { get; set; }

    public DateTime ScheduledArrivalUtc { get; set; }

    public int? GateId { get; set; }

    public int? AssignedAircraftId { get; set; }

    public byte Status { get; set; }

    public virtual Aircraft? AssignedAircraft { get; set; }

    public virtual Flight Flight { get; set; } = null!;

    public virtual Gate? Gate { get; set; }
}
