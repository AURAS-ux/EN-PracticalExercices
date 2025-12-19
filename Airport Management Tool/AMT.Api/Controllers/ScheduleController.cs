using AMT.Application.Dtos;
using AMT.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMT.Api.Controllers
{
    [Route("api/schedules")]
    [ApiController]
    public class ScheduleController(IScheduleService scheduleService) : ControllerBase
    {
        [HttpGet("{id}")]
        public IActionResult GetSchedule(int id)
        {
            var result = scheduleService.GetSchedule(id);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return StatusCode((int)result.StatusCode!, new { Errors = result.ErrorMessages });
        }

        [HttpGet("stats/upcoming")]
        public IActionResult GetUpcomingSchedules([FromQuery] string date)
        {
            var result = scheduleService.GetUpcomingSchedules(date);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return StatusCode((int)result.StatusCode!, new { Errors = result.ErrorMessages });
        }

        [HttpPost]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleDto scheduleRequest)
        {
            var result = await scheduleService.CreateScheduleAsync(scheduleRequest);
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetSchedule), new { id = result.Value!.Id }, result.Value);
            }
            return StatusCode((int)result.StatusCode!, new { Errors = result.ErrorMessages });
        }

        [HttpPost("import")]
        public async Task<IActionResult> BulkCreateSchedules([FromBody] string rawData)
        {
            var result = await scheduleService.BulkCreateSchedulesAsync(rawData);
            if (result.IsSuccess)
            {
                return Created("", result.Value);
            }
            return StatusCode((int)result.StatusCode!, new { Errors = result.ErrorMessages });
        }
    }
}
