using System;
using System.Net.Http;
using System.Threading.Tasks;
using FlightRecorder.Mvc.Configuration;
using FlightRecorder.Mvc.Entities;
using FlightRecorder.Mvc.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FlightRecorder.Mvc.Api
{
    public class SightingClient : FlightRecorderClientBase
    {
        private const string RouteKey = "Sightings";

        public SightingClient(HttpClient client, IOptions<AppSettings> settings, IHttpContextAccessor accessor, ICacheWrapper cache)
            : base(client, settings, accessor, cache)
        {
        }

        /// <summary>
        /// Create a new sighting
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Sighting> AddSightingAsync(DateTime date, int altitude, int aircraftId, int flightId, int locationId)
        {
            dynamic template = new
            {
                Date = date,
                AircraftId = aircraftId,
                Altitude = altitude,
                FlightId = flightId,
                LocationId = locationId
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            Sighting sighting = JsonConvert.DeserializeObject<Sighting>(json);
            return sighting;
        }
    }
}
