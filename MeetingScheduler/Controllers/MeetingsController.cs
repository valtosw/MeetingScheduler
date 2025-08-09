using MeetingScheduler.Application.DTOs;
using MeetingScheduler.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeetingScheduler.WebAPI.Controllers
{
    [Route("meetings")]
    [ApiController]
    public class MeetingsController(MeetingSchedulerService meetingSchedulerService) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(ValueTuple<DateTime, DateTime>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ScheduleMeeting([FromBody] MeetingSlotDto dto)
        {
            var meeting = meetingSchedulerService.ScheduleMeeting(dto);

            return meeting is null ? NotFound("No suitable time slot found.") : Ok(new {Start = meeting.StartTime, End = meeting.EndTime});
        }
    }
}
