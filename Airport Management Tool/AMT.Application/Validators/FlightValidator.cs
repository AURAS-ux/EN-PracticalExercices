using System;
using AMT.Domain.Models;
using FluentValidation;

namespace AMT.Application.Validators;

public class FlightValidator : AbstractValidator<Flight>
{
    public FlightValidator()
    {
        RuleFor(flight => flight.FlightNumber)
            .NotEmpty().WithMessage("Flight number is required.")
            .MaximumLength(8).WithMessage("Flight number cannot exceed 8 characters.");
        RuleFor(flight => flight)
            .Must(flight => flight.OriginAirport.Id != flight.DestinationAirport.Id)
            .WithMessage("Origin and destination airports must be different.");
        RuleFor(flight => flight.DefaultAircraft)
            .NotNull().WithMessage("Default aircraft is required.");
        RuleFor(flight => flight.Airline)
            .NotNull().WithMessage("Airline is required.");
    }
}