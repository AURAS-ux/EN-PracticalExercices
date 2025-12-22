using AMT.Application.Dtos;
using AMT.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMT.Api.Controllers
{
    [Route("api/airlines")]
    [ApiController]
    public class AirlineController(IAirlineService airlineService) : ControllerBase
    {
        [HttpGet("{id}")]
        public IActionResult GetAirlineById(int id)
        {
            var result = airlineService.GetAirlineByIdAsync(id);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return StatusCode((int)result.StatusCode!, result.ErrorMessages);
        }

        [HttpPost]
        [Authorize(Policy = "AirportAdmin")]
        public IActionResult CreateAirline(AirlineCreateRequestDto airlineCreateRequestDto)
        {
            var result = airlineService.CreateAirlineAsync(airlineCreateRequestDto).Result;
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetAirlineById), new { id = result.Value!.Id }, result.Value);
            }
            return StatusCode((int)result.StatusCode!, result.ErrorMessages);
        }

        [HttpGet]
        public IActionResult GetAllAirlines()
        {
            var result = airlineService.GetAllAirlinesAsync();
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return StatusCode((int)result.StatusCode!, result.ErrorMessages);
        }
    }
}
