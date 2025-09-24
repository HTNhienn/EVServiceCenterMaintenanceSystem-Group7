using EVServiceCenterMaintenanceSystem.Models;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace EVServiceCenterMaintenanceSystem.Services
{
    // Simple thread-safe in-memory store. Replace later with EF implementation.
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly ConcurrentDictionary<string, User> _users =
            new ConcurrentDictionary<string, User>(StringComparer.OrdinalIgnoreCase);

        public Task<User?> GetByUsernameAsync(string username)
        {
            _users.TryGetValue(username, out var user);
            return Task.FromResult(user);
        }

        public Task<bool> AddUserAsync(User user)
        {
            // TryAdd returns false if username already exists.
            var added = _users.TryAdd(user.Username, user);
            return Task.FromResult(added);
        }
    }
}
