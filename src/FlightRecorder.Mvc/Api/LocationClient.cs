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
    public class LocationClient : FlightRecorderClientBase
    {
        private const string RouteKey = "Locations";
        private const string CacheKey = "Locations";

        public LocationClient(HttpClient client, IOptions<AppSettings> settings, IHttpContextAccessor accessor, ICacheWrapper cache)
            : base(client, settings, accessor, cache)
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
                string route = @$"{Settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/1/{int.MaxValue}";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                locations = JsonConvert.DeserializeObject<List<Location>>(json, JsonSettings).OrderBy(m => m.Name).ToList();
                Cache.Set(CacheKey, locations, Settings.Value.CacheLifetimeSeconds);
            }

            // Extract and return the page of interest
            var page = locations.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return page;
        }

        /// <summary>
        /// Return the location with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Location> GetLocationAsync(int id)
        {
            Location location = Cache.Get<List<Location>>(CacheKey)?.FirstOrDefault(l => l.Id == id);
            if (location == null)
            {
                string route = @$"{Settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                location = JsonConvert.DeserializeObject<Location>(json, JsonSettings);
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
            Location location = JsonConvert.DeserializeObject<Location>(json, JsonSettings);
            return location;
        }

        /// <summary>
        /// Update an existing location
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Location> UpdateLocationAsync(int id, string name)
        {
            Cache.Remove(CacheKey);
            string data = $"\"{name}\"";
            string route = @$"{Settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
            string json = await SendDirectAsync(route, data, HttpMethod.Put);
            Location location = JsonConvert.DeserializeObject<Location>(json, JsonSettings);
            return location;
        }
    }
}
