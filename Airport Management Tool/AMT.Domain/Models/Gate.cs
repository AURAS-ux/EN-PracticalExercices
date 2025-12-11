namespace AMT.Domain.Models;

public class Gate
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public Airport Airport { get; set; } = null!;
}
