using System;

namespace AMT.Domain.Models;

public class Airport
{
    public int Id { get; set; }
    public string IATACode { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string Timezone { get; set; } = null!;
}
