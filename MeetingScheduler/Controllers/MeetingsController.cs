using MeetingScheduler.Application.DTOs;
using MeetingScheduler.Application.Services;
using MeetingScheduler.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace MeetingScheduler.WebAPI.Controllers
{
    [Route("meetings")]
    [ApiController]
    public class MeetingsController(MeetingSchedulerService meetingSchedulerService) : ControllerBase
    {
        [HttpPost("schedule")]
        [ProducesResponseType(typeof(Meeting), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ScheduleMeeting([FromBody] ScheduleMeetingDto dto)
        {
            var meeting = meetingSchedulerService.ScheduleMeeting(dto);

            return meeting is null ? NotFound("No suitable time slot found for the meeting.") : Ok(meeting);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ValueTuple<DateTime, DateTime>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult FindEarliestSlot([FromBody] FindSlotDto dto)
        {
            var slot = meetingSchedulerService.FindEarliestSlot(dto);

            return slot is null ? NotFound("No suitable time slot found.") : Ok(new {Start = slot.Value.start, End = slot.Value.end});
        }
    }
}
