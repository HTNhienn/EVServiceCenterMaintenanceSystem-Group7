using EVServiceCenterMaintenanceSystem.Models;
using System.Threading.Tasks;

namespace EVServiceCenterMaintenanceSystem.Services
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> AddUserAsync(User user);
    }
}
