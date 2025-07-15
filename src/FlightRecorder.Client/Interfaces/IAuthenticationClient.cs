using System.Threading.Tasks;

namespace FlightRecorder.Client.Interfaces
{
    public interface IAuthenticationClient
    {
        Task<string> AuthenticateAsync(string username, string password);
        void ClearAuthentication();
        void ClearCachedUserAttributes();
    }
}
