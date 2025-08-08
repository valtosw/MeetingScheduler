using MeetingScheduler.Application.Interfaces;
using MeetingScheduler.Domain.Models;

namespace MeetingScheduler.Infrastructure
{
    public class MeetingRepository : IMeetingRepository
    {
        private readonly List<Meeting> _meetings = new();
        
        public Meeting Add(Meeting meeting)
        {
            _meetings.Add(meeting);

            return meeting;
        }

        public List<Meeting> GetByUserId(int userId) => [.. _meetings.Where(m => m.ParticipantIds.Contains(userId))];

        public List<Meeting> GetAll() => _meetings;
    }
}
