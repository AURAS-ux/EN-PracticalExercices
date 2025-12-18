using AMT.Application.Dtos;
using AMT.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMT.Api.Controllers
{
    [Route("api/flights")]
    [ApiController]
    public class FlightsController(IFlightService flightService) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateFlight([FromBody] FlightRequest createFlightDto)
        {
            var result = await flightService.CreateFlightAsync(createFlightDto);
            if(result.IsSuccess)
            {
                return CreatedAtAction(nameof(CreateFlight), new { id = result.Value!.Id }, result.Value);
            }
            else
            {
                var errors = result.ErrorMessage;
                var exceptions = result.Exception;
                var statusCode = (int)result.StatusCode!;

                return StatusCode(statusCode, new
                {
                    Errors = errors,
                    Exceptions = exceptions?.Select(e => e.Message)
                });
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> FilterFlightsBy([FromQuery] string? date = null, [FromQuery] string? origin = null, [FromQuery] string? destination = null)
        {
            var result = await flightService.FilterFlightsBy(date, origin, destination);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            else
            {
                var errors = result.ErrorMessage;
                var exceptions = result.Exception;
                var statusCode = (int)result.StatusCode!;

                return StatusCode(statusCode, new
                {
                    Errors = errors,
                    Exceptions = exceptions?.Select(e => e.Message)
                });
            }
        }
    }
}
