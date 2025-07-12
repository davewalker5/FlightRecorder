using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FlightRecorder.Client.Interfaces;
using FlightRecorder.Entities.Config;
using FlightRecorder.Entities.Db;
using Microsoft.Extensions.Logging;

namespace FlightRecorder.Client.ApiClient
{
    public class SightingClient : FlightRecorderClientBase, ISightingClient
    {
        private const string RouteKey = "Sightings";

        public SightingClient(
            IFlightRecorderHttpClient client,
            FlightRecorderApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ICacheWrapper cache,
            ILogger<SightingClient> logger)
            : base(client, settings, tokenProvider, cache, logger)
        {
        }

        /// <summary>
        /// Retrieve a sighting given its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Sighting> GetSightingAsync(long id)
        {
            string route = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            Sighting sighting = Deserialize<Sighting>(json);
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
        /// <param name="isMyFlight"></param>
        /// <returns></returns>
        public async Task<Sighting> AddSightingAsync(DateTime date, long altitude, long aircraftId, long flightId, long locationId, bool isMyFlight)
        {
            dynamic template = new
            {
                Date = date,
                AircraftId = aircraftId,
                Altitude = altitude,
                FlightId = flightId,
                LocationId = locationId,
                IsMyFlight = isMyFlight
            };

            string data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            Sighting sighting = Deserialize<Sighting>(json);
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
        /// <param name="isMyFlight"></param>
        /// <returns></returns>
        public async Task<Sighting> UpdateSightingAsync(long id, DateTime date, long altitude, long aircraftId, long flightId, long locationId, bool isMyFlight)
        {
            dynamic template = new
            {
                Id = id,
                Date = date,
                AircraftId = aircraftId,
                Altitude = altitude,
                FlightId = flightId,
                LocationId = locationId,
                IsMyFlight = isMyFlight
            };

            string data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            Sighting sighting = Deserialize<Sighting>(json);
            return sighting;
        }
    }
}
