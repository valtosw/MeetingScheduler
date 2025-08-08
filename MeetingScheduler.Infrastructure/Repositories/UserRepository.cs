using MeetingScheduler.Application.Interfaces;
using MeetingScheduler.Domain.Models;

namespace MeetingScheduler.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly List<User> _users = [];

        public User Add(User user)
        {
            _users.Add(user);

            return user;
        }

        public User? GetById(int id) => _users.FirstOrDefault(u => u.Id == id);

        public List<User> GetAll() => _users;
    }
}
