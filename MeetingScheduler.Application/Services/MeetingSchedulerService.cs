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
            TimeSpan open = TimeSpan.FromHours(9);
            TimeSpan close = TimeSpan.FromHours(17);
            TimeSpan duration = TimeSpan.FromMinutes(dto.Duration);

            DateTime searchStart = dto.WindowStart;
            DateTime searchEnd = dto.WindowEnd;

            searchStart = AdjustToBusinessStart(searchStart, open, close);
            searchEnd = AdjustToBusinessEnd(searchEnd, open, close);

            if (searchStart >= searchEnd)
            {
                return null;
            }

            DateTime currentDay = searchStart.Date;

            while (currentDay <= searchEnd.Date)
            {
                DateTime dayStart = currentDay == searchStart.Date
                    ? searchStart
                    : currentDay.Add(open);
                DateTime dayEnd = currentDay == searchEnd.Date
                    ? searchEnd
                    : currentDay.Add(close);

                if (dayStart >= dayEnd)
                {
                    currentDay = currentDay.AddDays(1);
                    continue;
                }

                var busy = meetingRepository.GetAll()
                    .Where(m => m.ParticipantIds.Any(id => dto.ParticipantIds.Contains(id)))
                    .Where(m => m.EndTime > dayStart && m.StartTime < dayEnd)
                    .Select(m => (
                        Start: m.StartTime < dayStart ? dayStart : m.StartTime,
                        End: m.EndTime > dayEnd ? dayEnd : m.EndTime))
                    .OrderBy(m => m.Start)
                    .ToList();

                var merged = MergeIntervals(busy);

                DateTime indicator = dayStart;

                foreach (var (start, end) in merged)
                {
                    if (start - indicator >= duration)
                    {
                        return (indicator, indicator.Add(duration));
                    }

                    indicator = end;
                }

                if (dayEnd - indicator >= duration)
                {
                    return (indicator, indicator.Add(duration));
                }

                currentDay = currentDay.AddDays(1);
            }

            return null;
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

        private static DateTime AdjustToBusinessStart(DateTime dt, TimeSpan open, TimeSpan close)
        {
            if (dt.TimeOfDay < open)
            {
                return dt.Date.Add(open);
            }

            if (dt.TimeOfDay >= close)
            {
                return dt.Date.AddDays(1).Add(open);
            }

            return dt;
        }

        private static DateTime AdjustToBusinessEnd(DateTime dt, TimeSpan open, TimeSpan close)
        {
            if (dt.TimeOfDay > close)
            {
                return dt.Date.Add(close);
            }

            if (dt.TimeOfDay < open)
            {
                return dt.Date.Add(open);
            }

            return dt;
        }

        private static List<(DateTime Start, DateTime End)> MergeIntervals(List<(DateTime Start, DateTime End)> intervals)
        {
            var merged = new List<(DateTime Start, DateTime End)>();

            foreach (var interval in intervals)
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

            return merged;
        }
    }
}
