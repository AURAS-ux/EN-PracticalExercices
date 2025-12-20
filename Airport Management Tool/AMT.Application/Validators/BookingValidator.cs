using System;
using AMT.Domain.Models;
using FluentValidation;

namespace AMT.Application.Validators;

public class BookingValidator : AbstractValidator<Booking>
{
    public BookingValidator()
    {
        RuleFor(booking => booking)
        .Must(booking => booking.Quantity <= booking.Ticket.SeatInventory)
            .WithMessage("The requested quantity exceeds the available seat inventory.");
    }
}
