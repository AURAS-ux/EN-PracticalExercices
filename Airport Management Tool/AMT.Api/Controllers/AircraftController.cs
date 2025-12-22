using AMT.Application.Dtos;
using AMT.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMT.Api.Controllers
{
    [Route("api/aircrafts")]
    [ApiController]
    public class AircraftController(IAircraftService aircraftService) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllAircrafts()
        {
            var result = aircraftService.GetAllAircrafts();
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return StatusCode((int)result.StatusCode!, new { Errors = result.ErrorMessages });
        }

        [HttpGet("{id}")]
        public IActionResult GetAircraftById(int id)
        {
            var result = aircraftService.GetAircraftByIdAsync(id);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return StatusCode((int)result.StatusCode!, new { Errors = result.ErrorMessages });
        }

        [HttpPost]
        [Authorize(Policy = "AirportAdmin")]
        public IActionResult CreateAircraft([FromBody] AircraftCreateRequestDto aircraftCreateRequestDto)
        {
            var result = aircraftService.CreateAircraftAsync(aircraftCreateRequestDto).GetAwaiter().GetResult();
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetAircraftById), new { id = result.Value!.Id }, result.Value);
            }
            return StatusCode((int)result.StatusCode!, new { Errors = result.ErrorMessages });
        }
    }
}
