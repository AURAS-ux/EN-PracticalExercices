using System.ComponentModel.DataAnnotations;

namespace AMT.Domain.Models;

public class Airline
{
    public int Id { get; set; }
    [StringLength(2, MinimumLength = 2, ErrorMessage = "IATA code must be exactly 2 characters.")]
    public string Iatacode { get; set; } = null!;
    [StringLength(100, ErrorMessage = "Airline name cannot exceed 100 characters.")]
    public string Name { get; set; } = null!;

    public override bool Equals(object? obj)
    {
        if (obj is not Airline other)
            return false;

        return Iatacode == other.Iatacode &&
               Name == other.Name;
    }
}
