namespace AMT.Domain.Models;

public class Airline
{
    public int Id { get; set; }
    public string IATACode { get; set; } = null!;
    public string Name { get; set; } = null!;
}
