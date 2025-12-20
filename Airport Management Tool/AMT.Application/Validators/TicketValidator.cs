using System;
using AMT.Domain.Models;
using FluentValidation;

namespace AMT.Application.Validators;

public class TicketValidator : AbstractValidator<Ticket>
{
    public TicketValidator()
    {
        RuleFor(t => t.FareClass)
        .Must(fc => fc. Equals("Y", StringComparison.OrdinalIgnoreCase) ||
                    fc.Equals("J", StringComparison.OrdinalIgnoreCase) ||
                    fc.Equals("M", StringComparison.OrdinalIgnoreCase) ||
                    fc.Equals("F", StringComparison.OrdinalIgnoreCase)
                    )
        .WithMessage("FareClass must be either 'Y', 'J', 'M', or 'F'.");

        RuleFor(t => t.Currency)
        .Must(c => c.Length == 3)
        .WithMessage("Currency code must be exactly 3 characters.");
    }
}
