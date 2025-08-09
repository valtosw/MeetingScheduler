namespace MeetingScheduler.Application.DTOs
{
    public class MeetingSlotDto
    {
        public List<int> ParticipantIds { get; set; } = [];
        public int DurationMinutes { get; set; }
        public DateTime EarliestStart { get; set; }
        public DateTime LatestEnd { get; set; }
    }
}
