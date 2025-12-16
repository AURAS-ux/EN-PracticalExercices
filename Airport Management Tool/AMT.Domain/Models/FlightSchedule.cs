namespace AMT.Domain.Models;

public class FlightSchedule
{
    public int Id { get; set; }
    public Flight Flight { get; set; } = null!;
    public DateTime ScheduledDepartureUtc { get; set; }
    public DateTime ScheduledArrivalUtc { get; set; }
    public Gate Gate { get; set; } = null!;
    public Aircraft AssignedAircraft { get; set; } = null!;
    public ScheduleStatus Status { get; set; } = ScheduleStatus.PLANNED;
    
    public enum ScheduleStatus : byte
    {
        PLANNED = 0,
        BOARDING = 1,
        DEPARTED = 2,
        CANCELLED = 3,
        DELAYED = 4
    }
}
