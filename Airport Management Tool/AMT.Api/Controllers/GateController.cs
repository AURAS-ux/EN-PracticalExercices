using System.Threading.Tasks;
using AMT.Application.Dtos;
using AMT.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMT.Api.Controllers
{
    [Route("api/gates")]
    [ApiController]
    public class GateController(IGateService gateService) : ControllerBase
    {
        [HttpGet("{id}")]
        public IActionResult GetGateById(int id)
        {
            var result = gateService.GetGateById(id);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return StatusCode((int)result.StatusCode!, new { Errors = result.ErrorMessages });
        }

        [HttpPost]
        [Authorize(Policy = "AirportAdmin")]
        public async Task<IActionResult> CreateGate([FromBody] GateCreateRequestDto gateCreateRequestDto)
        {
            var result = await gateService.CreateGate(gateCreateRequestDto);
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetGateById), new { id = result.Value!.Id }, result.Value);
            }
            return StatusCode((int)result.StatusCode!, new { Errors = result.ErrorMessages });
        }

        [HttpGet]
        public IActionResult GetAllGates()
        {
            var result = gateService.GetAllGates();
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return StatusCode((int)result.StatusCode!, new { Errors = result.ErrorMessages });
        }
    }
}
