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
    public class ManufacturerClient : FlightRecorderClientBase, IManufacturerClient
    {
        private const string RouteKey = "Manufacturers";
        private const string CacheKey = "Manufacturers";

        public ManufacturerClient(
            IFlightRecorderHttpClient client,
            FlightRecorderApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ICacheWrapper cache,
            ILogger<ManufacturerClient> logger)
            : base(client, settings, tokenProvider, cache, logger)
        {
        }

        /// <summary>
        /// Return a list of manufacturers
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Manufacturer>> GetManufacturersAsync(int pageNumber, int pageSize)
        {
            // Attempt to get the list of manufacturers from the cache
            List<Manufacturer> manufacturers = Cache.Get<List<Manufacturer>>(CacheKey);
            if (manufacturers == null)
            {
                // Not cached, so retrieve them from the service and cache them
                string route = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}/1/{int.MaxValue}";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                manufacturers = Deserialize<List<Manufacturer>>(json)?.OrderBy(m => m.Name).ToList();
                Cache.Set(CacheKey, manufacturers, Settings.CacheLifetimeSeconds);
            }

            // Extract and return the page of interest
            var page = manufacturers.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return page;
        }

        /// <summary>
        /// Return the manufacturer with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Manufacturer> GetManufacturerAsync(long id)
        {
            Manufacturer manufacturer = Cache.Get<List<Manufacturer>>(CacheKey)?.FirstOrDefault(l => l.Id == id);
            if (manufacturer == null)
            {
                string route = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                manufacturer = Deserialize<Manufacturer>(json);
            }

            return manufacturer;
        }

        /// <summary>
        /// Create a new manufacturer
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Manufacturer> AddManufacturerAsync(string name)
        {
            Cache.Remove(CacheKey);
            string data = $"\"{name}\"";
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            Manufacturer manufacturer = Deserialize<Manufacturer>(json);
            return manufacturer;
        }

        /// <summary>
        /// Update an existing manufacturer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Manufacturer> UpdateManufacturerAsync(long id, string name)
        {
            Cache.Remove(CacheKey);
            string data = $"\"{name}\"";
            string route = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
            string json = await SendDirectAsync(route, data, HttpMethod.Put);
            Manufacturer manufacturer = Deserialize<Manufacturer>(json);
            return manufacturer;
        }
    }
}