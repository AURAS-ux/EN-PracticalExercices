using System.ComponentModel.DataAnnotations;

namespace AMT.Domain.Models;

public class Gate
{
    public int Id { get; set; }
    [StringLength(10, ErrorMessage = "Gate code cannot exceed 10 characters.")]
    public string Code { get; set; } = null!;
    public Airport Airport { get; set; } = null!;
    public virtual IList<FlightSchedule> FlightSchedules { get; set; } = null!;
}
