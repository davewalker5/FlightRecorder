using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FlightRecorder.Client.Interfaces
{
    public interface IFlightRecorderHttpClient
    {
        Task<HttpResponseMessage> GetAsync(string uri, string token, ILogger logger);
        Task<HttpResponseMessage> PostAsync(string uri, HttpContent content, string token, ILogger logger);
        Task<HttpResponseMessage> PutAsync(string uri, HttpContent content, string token, ILogger logger);
        Task<HttpResponseMessage> DeleteAsync(string uri, string token, ILogger logger);
    }
}