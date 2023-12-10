using System.Threading.Tasks;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Api.Interfaces
{
    public interface IUserService
    {
        Task<User> AddUserAsync(string userName, string password);
        Task<string> AuthenticateAsync(string userName, string password);
        Task SetUserPasswordAsync(string userName, string password);
    }
}