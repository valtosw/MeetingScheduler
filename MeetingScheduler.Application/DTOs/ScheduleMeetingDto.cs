namespace MeetingScheduler.Application.DTOs
{
    public class ScheduleMeetingDto
    {
        public List<int> ParticipantIds { get; set; } = [];
        public int DurationMinutes { get; set; }
        public DateTime EarliestStart { get; set; }
        public DateTime LatestEnd { get; set; }
    }
}
