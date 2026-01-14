using System;
using System.ComponentModel.DataAnnotations;

namespace AMT.Application.Dtos;

public record CreateScheduleDto
{
    [Required(ErrorMessage = "Flight Id field is required")]
    public int FlightId { get; set; }
    [Required(ErrorMessage = "Missing departure UTC")]
    public required string ScheduleDepartureUtc { get; set; }
    [Required(ErrorMessage = "Missing arrival UTC")]
    public required string ScheduleArrivalUtc { get; set; }
    [Required(ErrorMessage = "Missing gate code")]
    public required string GateCode { get; set; }
    [Required(ErrorMessage = "Missing aircraft tail")]
    public required string AssignedAircraftTail { get; set; }
    public byte Status { get; set; } = 0;
}
