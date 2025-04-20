using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IFlightRecorderHttpClient
    {
        void SetHeaders(Dictionary<string, string> headers);
        Task<HttpResponseMessage> GetAsync(string uri);
    }
}