using System;
using AMT.Domain.Models;
using FluentValidation;

namespace AMT.Application.Validators;

public class ScheduleValidator : AbstractValidator<FlightSchedule>
{
    public ScheduleValidator()
    {
        RuleFor(schedule => schedule.ScheduledDepartureUtc)
            .LessThan(schedule => schedule.ScheduledArrivalUtc)
            .WithMessage("Scheduled departure time must be earlier than scheduled arrival time.");
    }
}
