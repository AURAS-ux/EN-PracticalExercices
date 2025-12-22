using AMT.Application.Dtos;
using AMT.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMT.Api.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingController(IBookingService bookingService) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto request)
        {
            var result = await bookingService.CreateBookingAsync(request);
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetBookingById), new { code = result.Value!.ConfirmationCode }, result.Value);
            }
            return StatusCode((int)result.StatusCode!, new { Errors = result.ErrorMessages! });
        }

        [HttpGet("{code}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetBookingById(int code)
        {
            var result = bookingService.GetBookingByCode(code);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return StatusCode((int)result.StatusCode!, new { Errors = result.ErrorMessages! });
        }

        [HttpDelete("{code}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteBooking(int code)
        {
            var result = await bookingService.DeleteBookingAsync(code);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return StatusCode((int)result.StatusCode!, new { Errors = result.ErrorMessages! });
        }
    }
}
