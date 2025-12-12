using System;
using System.Collections.Generic;

namespace AMT.Infrastructure.Entities;

public partial class Airline
{
    public int Id { get; set; }

    public string Iatacode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<Aircraft> Aircraft { get; set; } = new List<Aircraft>();

    public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();
}
