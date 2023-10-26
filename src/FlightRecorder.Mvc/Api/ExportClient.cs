using FlightRecorder.Mvc.Configuration;
using FlightRecorder.Mvc.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlightRecorder.Mvc.Api
{
    public class ExportClient : FlightRecorderClientBase
    {
        public ExportClient(HttpClient client,
            IOptions<AppSettings> settings,
            IHttpContextAccessor accessor,
            ICacheWrapper cache)
            : base(client, settings, accessor, cache)
        {
        }

        /// <summary>
        /// Trigger a sightings export via the API
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task ExportSightings(string fileName)
        {
            dynamic template = new { FileName = fileName };
            string data = JsonConvert.SerializeObject(template);
            await SendIndirectAsync("ExportSightings", data, HttpMethod.Post);
        }

        /// <summary>
        /// Trigger an airports export via the API
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task ExportAirports(string fileName)
        {
            dynamic template = new { FileName = fileName };
            string data = JsonConvert.SerializeObject(template);
            await SendIndirectAsync("ExportAirports", data, HttpMethod.Post);
        }
    }
}
