using System.Threading.Tasks;
using AMT.Application.Dtos;
using AMT.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMT.Api.Controllers
{
    [Route("api/tickets")]
    [ApiController]
    public class TicketsController(ITicketService ticketService) : ControllerBase
    {

        [HttpGet("by-flight/{flightId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetTicket(int flightId)
        {
            var ticketResult = ticketService.GetTicketByFlightId(flightId);
            if (ticketResult.IsSuccess)
            {
                return Ok(ticketResult.Value);
            }
            return StatusCode((int)ticketResult.StatusCode!, new { Errors = ticketResult.ErrorMessages });
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto createTicketDto)
        {
            var result = await ticketService.CreateTicket(createTicketDto);
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetTicket), new { flightId = result.Value!.Flight.Id }, result.Value);
            }
            return StatusCode((int)result.StatusCode!, new { Errors = result.ErrorMessages });
        }

        [HttpPut("{id}/inventory")]
        public async Task<IActionResult> SetSeatInventory(int id, [FromQuery] int inventory)
        {
            var updateDto = new UpdateTicketInventoryDto
            {
                TicketId = id,
                NewSeatInventory = inventory
            };
            var result = await ticketService.UpdateTicketInventory(updateDto);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return StatusCode((int)result.StatusCode!, new { Errors = result.ErrorMessages });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            var result = await ticketService.DeleteTicketAsync(id);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return StatusCode((int)result.StatusCode!, new { Errors = result.ErrorMessages });
        }
    }
}
