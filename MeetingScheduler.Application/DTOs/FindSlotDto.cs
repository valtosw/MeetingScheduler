namespace MeetingScheduler.Application.DTOs
{
    public class FindSlotDto
    {
        public IEnumerable<int> ParticipantIds { get; set; } = [];
        public int Duration { get; set; }
        public DateTime WindowStart { get; set; }
        public DateTime WindowEnd { get; set; }
    }
}
