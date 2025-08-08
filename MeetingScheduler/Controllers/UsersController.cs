using MeetingScheduler.Application.DTOs;
using MeetingScheduler.Application.Services;
using MeetingScheduler.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace MeetingScheduler.WebAPI.Controllers
{
    [Route("users")]
    [ApiController]
    public class UsersController(UserService userService, MeetingSchedulerService meetingSchedulerService) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public IActionResult CreateUser([FromBody] CreateUserDto dto)
        {
            var user = userService.CreateUser(dto.Name);

            return Ok(user);
        }

        [HttpGet("{userId}/meetings")]
        [ProducesResponseType(typeof(List<Meeting>), StatusCodes.Status200OK)]
        public IActionResult GetUserMeetings(int userId)
        {
            var meetings = meetingSchedulerService.GetMeetingsByUserId(userId);

            return Ok(meetings);
        }
    }
}
