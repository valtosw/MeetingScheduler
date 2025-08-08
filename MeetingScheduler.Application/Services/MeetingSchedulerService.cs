using MeetingScheduler.Application.DTOs;
using MeetingScheduler.Application.Interfaces;
using MeetingScheduler.Domain.Models;

namespace MeetingScheduler.Application.Services
{
    public class MeetingSchedulerService(IMeetingRepository meetingRepository)
    {
        private int _meetingIdCounter = 1;

        public (DateTime start, DateTime end)? FindEarliestSlot(FindSlotDto dto)
        {
            var existingMeetings = meetingRepository.GetAll();
            
            var busy = existingMeetings
                .Where(m => m.EndTime > dto.WindowStart &&
                            m.StartTime < dto.WindowEnd &&
                            m.ParticipantIds.Any(id => dto.ParticipantIds.Contains(id)))
                .Select(m => (
                    Start: m.StartTime < dto.WindowStart ? dto.WindowStart : m.StartTime,
                    End: m.EndTime > dto.WindowEnd ? dto.WindowEnd : m.EndTime))
                .OrderBy(m => m.Start)
                .ToList();

            var merged = new List<(DateTime Start, DateTime End)>();

            foreach (var interval in busy)
            {
                if (merged.Count == 0 || interval.Start > merged[^1].End)
                {
                    merged.Add(interval);
                }
                else
                {
                    merged[^1] = (merged[^1].Start, interval.End > merged[^1].End ? interval.End : merged[^1].End);
                }
            }

            DateTime indicator = dto.WindowStart;

            foreach (var (start, end) in merged)
            {
                if (start - indicator >= TimeSpan.FromMinutes(dto.Duration))
                {
                    return (indicator, indicator.Add(TimeSpan.FromMinutes(dto.Duration)));
                }

                indicator = end;
            }

            return dto.WindowEnd - indicator >= TimeSpan.FromMinutes(dto.Duration) ? (indicator, indicator.Add(TimeSpan.FromMinutes(dto.Duration))) : null;
        }

        public List<Meeting> GetMeetingsByUserId(int userId) => [.. meetingRepository.GetByUserId(userId)];

        public Meeting? ScheduleMeeting(ScheduleMeetingDto dto)
        {
            var findSlotDto = new FindSlotDto
            {
                ParticipantIds = dto.ParticipantIds,
                Duration = dto.DurationMinutes,
                WindowStart = dto.EarliestStart,
                WindowEnd = dto.LatestEnd
            };

            var slot = FindEarliestSlot(findSlotDto);

            return slot is null ? null : CreateAndSaveMeeting(slot.Value.start, dto);
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
