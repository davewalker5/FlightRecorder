using System.Collections.Generic;
using System.Threading.Tasks;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IUserManager
    {
        Task<User> AddUserAsync(string userName, string password);
        Task<bool> AuthenticateAsync(string userName, string password);
        Task DeleteUserAsync(string userName);
        Task<User> GetUserAsync(int userId);
        Task<User> GetUserAsync(string userName);
        IAsyncEnumerable<User> GetUsersAsync();
        Task SetPasswordAsync(string userName, string password);
    }
}