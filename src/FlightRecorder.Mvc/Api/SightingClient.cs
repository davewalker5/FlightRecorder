using System;
using System.Linq;
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
        /// Retrieve a sighting given its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Sighting> GetSightingAsync(int id)
        {
            string route = @$"{_settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            Sighting sighting = JsonConvert.DeserializeObject<Sighting>(json);
            return sighting;
        }

        /// <summary>
        /// Create a new sighting
        /// </summary>
        /// <param name="date"></param>
        /// <param name="altitude"></param>
        /// <param name="aircraftId"></param>
        /// <param name="flightId"></param>
        /// <param name="locationId"></param>
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

        /// <summary>
        /// Update an existing sighting
        /// </summary>
        /// <param name="id"></param>
        /// <param name="date"></param>
        /// <param name="altitude"></param>
        /// <param name="aircraftId"></param>
        /// <param name="flightId"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public async Task<Sighting> UpdateSightingAsync(int id, DateTime date, int altitude, int aircraftId, int flightId, int locationId)
        {
            dynamic template = new
            {
                Id = id,
                Date = date,
                AircraftId = aircraftId,
                Altitude = altitude,
                FlightId = flightId,
                LocationId = locationId
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            Sighting sighting = JsonConvert.DeserializeObject<Sighting>(json);
            return sighting;
        }
    }
}
