using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FlightRecorder.Client.Interfaces;
using FlightRecorder.Entities.Config;
using FlightRecorder.Entities.Db;
using Microsoft.Extensions.Logging;


namespace FlightRecorder.Client.ApiClient
{
    public class LocationClient : FlightRecorderClientBase, ILocationClient
    {
        private const string RouteKey = "Locations";
        private const string CacheKey = "Locations";

        public LocationClient(
            IFlightRecorderHttpClient client,
            FlightRecorderApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ICacheWrapper cache,
            ILogger<LocationClient> logger)
            : base(client, settings, tokenProvider, cache, logger)
        {
        }

        /// <summary>
        /// Return a list of locations
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Location>> GetLocationsAsync(int pageNumber, int pageSize)
        {
            // Attempt to get the list of locations from the cache
            List<Location> locations = Cache.Get<List<Location>>(CacheKey);
            if (locations == null)
            {
                // Not cached, so retrieve them from the service and cache them
                string route = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}/1/{int.MaxValue}";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                locations = Deserialize<List<Location>>(json)?.OrderBy(m => m.Name).ToList();
                Cache.Set(CacheKey, locations, Settings.CacheLifetimeSeconds);
            }

            // Extract and return the page of interest
            var page = locations?.Skip((pageNumber - 1) * pageSize)?.Take(pageSize)?.ToList();
            return page;
        }

        /// <summary>
        /// Return the location with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Location> GetLocationAsync(long id)
        {
            Location location = Cache.Get<List<Location>>(CacheKey)?.FirstOrDefault(l => l.Id == id);
            if (location == null)
            {
                string route = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                location = Deserialize<Location>(json);
            }

            return location;
        }

        /// <summary>
        /// Create a new location
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Location> AddLocationAsync(string name)
        {
            Cache.Remove(CacheKey);
            string data = $"\"{name}\"";
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            Location location = Deserialize<Location>(json);
            return location;
        }

        /// <summary>
        /// Update an existing location
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Location> UpdateLocationAsync(long id, string name)
        {
            Cache.Remove(CacheKey);
            string data = $"\"{name}\"";
            string route = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
            string json = await SendDirectAsync(route, data, HttpMethod.Put);
            Location location = Deserialize<Location>(json);
            return location;
        }

        /// <summary>
        /// Delete an existing location
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task DeleteLocationAsync(long id)
        {
            Cache.Remove(CacheKey);
            string route = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
            _ = await SendDirectAsync(route, null, HttpMethod.Delete);
        }
    }
}
