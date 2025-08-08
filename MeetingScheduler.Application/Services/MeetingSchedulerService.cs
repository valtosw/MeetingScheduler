using MeetingScheduler.Application.DTOs;
using MeetingScheduler.Application.Interfaces;
using MeetingScheduler.Domain.Models;

namespace MeetingScheduler.Application.Services
{
    public class MeetingSchedulerService(IMeetingRepository meetingRepository)
    {
        private int _meetingIdCounter = 1;

        public Meeting? ScheduleMeeting(ScheduleMeetingDto dto)
        {
            var allMeetings = dto.ParticipantIds
                .SelectMany(userId => meetingRepository.GetByUserId(userId))
                .Where(m => m.EndTime > dto.EarliestStart && m.StartTime < dto.LatestEnd)
                .OrderBy(m => m.StartTime)
                .ToList();

            DateTime current = dto.EarliestStart;

            foreach (var meeting in allMeetings)
            {
                if ((meeting.StartTime - current).TotalMinutes >= dto.DurationMinutes)
                {
                    return CreateAndSaveMeeting(current, dto);
                }

                current = current > meeting.EndTime ? current : meeting.EndTime;
            }

            if ((dto.LatestEnd - current).TotalMinutes >= dto.DurationMinutes)
            {
                return CreateAndSaveMeeting(current, dto);
            }

            return null;
        }

        private Meeting CreateAndSaveMeeting(DateTime start, ScheduleMeetingDto dto)
        {
            var meeting = new Meeting
            {
                Id = _meetingIdCounter++,
                ParticipantIds = dto.ParticipantIds,
                StartTime = start,
                EndTime = start.AddMinutes(dto.DurationMinutes)
            };

            return meetingRepository.Add(meeting);
        }
    }
}
