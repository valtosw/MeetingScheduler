using MeetingScheduler.Domain.Models;

namespace MeetingScheduler.Application.Interfaces
{
    public interface IUserRepository
    {
        User Add(User user);
        User? GetById(int id);
        List<User> GetAll();
    }
}
