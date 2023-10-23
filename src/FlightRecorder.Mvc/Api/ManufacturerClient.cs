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
    public class ManufacturerClient : FlightRecorderClientBase
    {
        private const string RouteKey = "Manufacturers";
        private const string CacheKey = "Manufacturers";
        private const int AllManufacturersPageSize = 1000000;

        public ManufacturerClient(HttpClient client, IOptions<AppSettings> settings, IHttpContextAccessor accessor, ICacheWrapper cache)
            : base(client, settings, accessor, cache)
        {
        }

        /// <summary>
        /// Return a list of manufacturers
        /// </summary>
        /// <returns></returns>
        public async Task<List<Manufacturer>> GetManufacturersAsync()
        {
            List<Manufacturer> manufacturers = Cache.Get<List<Manufacturer>>(CacheKey);
            if (manufacturers == null)
            {
                string route = @$"{Settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/1/{AllManufacturersPageSize}";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                manufacturers = JsonConvert.DeserializeObject<List<Manufacturer>>(json, JsonSettings).OrderBy(m => m.Name).ToList();
                Cache.Set(CacheKey, manufacturers, Settings.Value.CacheLifetimeSeconds);
            }
            return manufacturers;
        }

        /// <summary>
        /// Return the manufacturer with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Manufacturer> GetManufacturerAsync(int id)
        {
            Manufacturer manufacturer = Cache.Get<List<Manufacturer>>(CacheKey)?.FirstOrDefault(l => l.Id == id);
            if (manufacturer == null)
            {
                string route = @$"{Settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                manufacturer = JsonConvert.DeserializeObject<Manufacturer>(json, JsonSettings);
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
            Manufacturer manufacturer = JsonConvert.DeserializeObject<Manufacturer>(json, JsonSettings);
            return manufacturer;
        }

        /// <summary>
        /// Update an existing manufacturer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Manufacturer> UpdateManufacturerAsync(int id, string name)
        {
            Cache.Remove(CacheKey);
            string data = $"\"{name}\"";
            string route = @$"{Settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
            string json = await SendDirectAsync(route, data, HttpMethod.Put);
            Manufacturer manufacturer = JsonConvert.DeserializeObject<Manufacturer>(json, JsonSettings);
            return manufacturer;
        }
    }
}