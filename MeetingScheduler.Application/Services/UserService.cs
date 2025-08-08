using MeetingScheduler.Application.Interfaces;
using MeetingScheduler.Domain.Models;

namespace MeetingScheduler.Application.Services
{
    public class UserService(IUserRepository userRepository)
    {
        private int _userIdCounter = 1;

        public User CreateUser(string name)
        {
            var user = new User
            {
                Id = _userIdCounter++,
                Name = name
            };

            return userRepository.Add(user);
        }
    }
}
