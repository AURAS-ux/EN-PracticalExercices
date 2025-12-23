using AMT.Api.Utils.Wrappers;
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
        private const int MaxFileSizeInBytes = 2 * 1024 * 1024; // 2 MB

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
        public async Task<IActionResult> BulkCreateSchedules([FromForm] FileUploadDto fileUpload)
        {
            if (fileUpload.File == null || fileUpload.File.Length == 0)
            {
                return BadRequest(new { Errors = new[] { "No file uploaded or file is empty." } });
            }

            if(fileUpload.File.Length > MaxFileSizeInBytes)
            {
                return BadRequest(new { Errors = new[] { "File size exceeds the 2MB limit." } });
            }

            if(fileUpload.File.ContentType != "application/json")
            {
                return BadRequest(new { Errors = new[] { "Invalid file type. Only JSON files are accepted." } });
            }

            var result = await scheduleService.BulkCreateSchedulesAsync(fileUpload.File.OpenReadStream());
            bool allSuccess = result.ImportResults.All(r => r.Value != null && r.Value.IsSuccess);
            bool allFailed = result.ImportResults.All(r => r.Value != null && !r.Value.IsSuccess);
            if (allSuccess)
            {
                return CreatedAtAction(nameof(BulkCreateSchedules), result);
            }
            return StatusCode(!allSuccess && !allFailed ? StatusCodes.Status207MultiStatus : StatusCodes.Status400BadRequest, result);
        }


        [HttpGet("flights")]
        public IActionResult FilterFlights([FromQuery] string? origin = null, [FromQuery] string? destination = null, [FromQuery] string? date = null)
        {
            var result = scheduleService.FilterFlights(origin, destination, date);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return StatusCode((int)result.StatusCode!, new { Errors = result.ErrorMessages });
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAllSchedules()
        {
            var result = scheduleService.GetAllSchedules();
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return StatusCode((int)result.StatusCode!, new { Errors = result.ErrorMessages! });
        }
    }
}
