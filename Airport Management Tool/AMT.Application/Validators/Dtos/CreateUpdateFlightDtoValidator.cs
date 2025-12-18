using System;
using AMT.Application.Dtos;
using FluentValidation;

namespace AMT.Application.Validators.Dtos;

public class CreateUpdateFlightDtoValidator : AbstractValidator<CreateFlightDto>
{
    public CreateUpdateFlightDtoValidator()
    {
        RuleFor(f => f.FlightNumber)
            .NotEmpty().WithMessage("Flight number is required.")
            .MaximumLength(10).WithMessage("Flight number cannot exceed 10 characters.");

        RuleFor(f => f.AirlineIata)
            .NotEmpty().WithMessage("Airline IATA code is required.")
            .Length(2).WithMessage("Airline IATA code must be exactly 2 characters.");

        RuleFor(f => f.OriginIata)
            .NotEmpty().WithMessage("Origin airport IATA code is required.")
            .Length(3).WithMessage("Origin airport IATA code must be exactly 3 characters.");

        RuleFor(f => f.DestinationIata)
            .NotEmpty().WithMessage("Destination airport IATA code is required.")
            .Length(3).WithMessage("Destination airport IATA code must be exactly 3 characters.");
        RuleFor(f => f).Must(f => f.OriginIata != f.DestinationIata)
            .WithMessage("Origin and destination airports must be different.");
    }
}
