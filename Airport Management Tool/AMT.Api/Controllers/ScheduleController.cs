using AMT.Application.Dtos;
using AMT.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace AMT.Api.Controllers
{
    [Route("api/schedules")]
    [ApiController]
    public class ScheduleController(IScheduleService scheduleService) : ControllerBase
    {
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status207MultiStatus)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BulkCreateSchedules([FromForm] ScheduleImportRequest request)
        {
            var file = request.File;
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Errors = new[] { "No file uploaded or file is empty." } });
            }

            string rawData;
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                rawData = await reader.ReadToEndAsync();
            }
            var result = await scheduleService.BulkCreateSchedulesAsync(rawData);
            bool allSuccess = result.ImportResults.All(r => r.Value != null && r.Value.IsSuccess);
            if (allSuccess)
            {
                return CreatedAtAction(nameof(BulkCreateSchedules), result);
            }
            return StatusCode(StatusCodes.Status207MultiStatus, result);
        }
    }

    public class ScheduleImportRequest
{
    public IFormFile File { get; set; } = null!;
}
}
