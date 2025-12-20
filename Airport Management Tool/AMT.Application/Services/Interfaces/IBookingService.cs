using System;
using AMT.Application.Dtos;
using AMT.Domain.Models;
using AMT.Domain.Utils;

namespace AMT.Application.Services.Interfaces;

public interface IBookingService
{
    Task<Result<BookingCreatedDto, Exception>> CreateBookingAsync(CreateBookingDto bookingDto);
    Result<Booking, Exception> GetBookingByCode(int bookingCode);
    Task<Result<string, Exception>> DeleteBookingAsync(int bookingId);
}
