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
        public async Task<IActionResult> CreateFlight([FromBody] CreateFlightDto createFlightDto)
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

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateFlight(int id, [FromBody] UpdateFlightDto updateFlightDto)
        {
            var result = await flightService.UpdateFlightAsync(id, updateFlightDto);
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

        [HttpDelete("{flightId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteFligt(int flightId)
        {
            var result = await flightService.DeleteFlightAsync(flightId);
            if (result.IsSuccess)
            {
                return NoContent();
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
