using System.Threading.Tasks;
using AMT.Application.Dtos;
using AMT.Application.Services.Interfaces;
using AMT.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMT.Api.Controllers
{
    [Route("api/airports")]
    [ApiController]
    public class AirportController(IAirportService service) : ControllerBase
    {
        [HttpGet("{id}")]
        [Authorize(Policy = "AirportAdmin")]
        public IActionResult GetAirportById(int id)
        {
            var result = service.GetAirportById(id);
            if (!result.IsSuccess)
            {
                return StatusCode((int)result.StatusCode!, new { Errors = result.ErrorMessages });
            }
            return Ok(result.Value);
        }
        [HttpGet]
        public IActionResult GetAllAirports()
        {
            var result = service.GetAllAirports();
            if (!result.IsSuccess)
            {
                return StatusCode((int)result.StatusCode!, new { Errors = result.ErrorMessages });
            }
            return Ok(result.Value);
        }

        [HttpPost]
        [Authorize(Policy = "AirportAdmin")]
        public async Task<IActionResult> CreateAirport([FromBody] AirportCreateRequestDto airportCreateRequestDto)
        {
            var result = await service.CreateAirportAsync(airportCreateRequestDto);
            if (!result.IsSuccess)
            {
                return StatusCode((int)result.StatusCode!, new { Errors = result.ErrorMessages });
            }
            return CreatedAtAction(nameof(GetAirportById), new { id = result.Value!.Id }, result.Value);
        }
    }
}
