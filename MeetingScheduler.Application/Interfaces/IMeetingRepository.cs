using MeetingScheduler.Domain.Models;

namespace MeetingScheduler.Application.Interfaces
{
    public interface IMeetingRepository
    {
        Meeting Add(Meeting meeting);
        List<Meeting> GetByUserId(int userId);
        List<Meeting> GetAll();
    }
}
