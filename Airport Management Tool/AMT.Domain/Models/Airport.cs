using System;
using System.ComponentModel.DataAnnotations;

namespace AMT.Domain.Models;

public class Airport
{
    public int Id { get; set; }
    [StringLength(3, MinimumLength = 3, ErrorMessage = "IATA code must be exactly 3 characters.")]
    public string IATACode { get; set; } = null!;
    [StringLength(120, ErrorMessage = "Airport name cannot exceed 120 characters.")]
    public string Name { get; set; } = null!;
    [StringLength(80, ErrorMessage = "City name cannot exceed 80 characters.")]
    public string City { get; set; } = null!;
    [StringLength(80, ErrorMessage = "Country name cannot exceed 80 characters.")]
    public string Country { get; set; } = null!;
    public string Timezone { get; set; } = null!;
    public virtual IList<Flight> FlightDestinationAirports { get; set; } = null!;

    public virtual IList<Flight> FlightOriginAirports { get; set; } = null!;

    public virtual IList<Gate> Gates { get; set; } = null!;
}
