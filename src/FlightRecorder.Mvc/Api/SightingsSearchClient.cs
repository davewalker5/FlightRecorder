using System.Collections.Generic;
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
    public class SightingsSearchClient : FlightRecorderClientBase
    {
        private const string RouteKey = "Sightings";
        private const string CacheKeyPrefix = "Sightings";

        public SightingsSearchClient(HttpClient client, IOptions<AppSettings> settings, IHttpContextAccessor accessor, ICacheWrapper cache)
            : base(client, settings, accessor, cache)
        {
        }

        /// <summary>
        /// Return the specified page of sightings filtered by route
        /// </summary>
        /// <param name="embarkation"></param>
        /// <param name="destination"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Sighting>> GetSightingsByRoute(string embarkation, string destination, int page, int pageSize)
        {
            string baseRoute = _settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route;
            string route = $"{baseRoute}/route/{embarkation}/{destination}/{page}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            List<Sighting> sightings = JsonConvert.DeserializeObject<List<Sighting>>(json);
            return sightings;
        }

        /// <summary>
        /// Return the specified page of sightings filtered by flight number
        /// </summary>
        /// <param name="number"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Sighting>> GetSightingsByFlight(string number, int page, int pageSize)
        {
            string baseRoute = _settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route;
            string route = $"{baseRoute}/flight/{number}/{page}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            List<Sighting> sightings = JsonConvert.DeserializeObject<List<Sighting>>(json);
            return sightings;
        }

        /// <summary>
        /// Return the specified page of sightings filtered by airline
        /// </summary>
        /// <param name="airlineId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Sighting>> GetSightingsByAirline(int airlineId, int page, int pageSize)
        {
            string baseRoute = _settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route;
            string route = $"{baseRoute}/airline/{airlineId}/{page}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            List<Sighting> sightings = JsonConvert.DeserializeObject<List<Sighting>>(json);
            return sightings;
        }
    }
}
