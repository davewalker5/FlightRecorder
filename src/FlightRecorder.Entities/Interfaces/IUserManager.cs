using System.Collections.Generic;
using System.Threading.Tasks;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IUserManager
    {
        User AddUser(string userName, string password);
        Task<User> AddUserAsync(string userName, string password);
        bool Authenticate(string userName, string password);
        Task<bool> AuthenticateAsync(string userName, string password);
        void DeleteUser(string userName);
        Task DeleteUserAsync(string userName);
        User GetUser(int userId);
        User GetUser(string userName);
        Task<User> GetUserAsync(int userId);
        Task<User> GetUserAsync(string userName);
        IEnumerable<User> GetUsers();
        IAsyncEnumerable<User> GetUsersAsync();
        void SetPassword(string userName, string password);
        Task SetPasswordAsync(string userName, string password);
    }
}