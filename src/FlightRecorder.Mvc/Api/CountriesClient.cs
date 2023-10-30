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
    public class CountriesClient : FlightRecorderClientBase
    {
        private const string RouteKey = "Countries";
        private const string CacheKey = "Countries";
        private const int AllCountriesPageSize = 1000000;

        public CountriesClient(HttpClient client, IOptions<AppSettings> settings, IHttpContextAccessor accessor, ICacheWrapper cache)
            : base(client, settings, accessor, cache)
        {
        }

        /// <summary>
        /// Return a list of countries
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Country>> GetCountriesAsync(int pageNumber, int pageSize)
        {
            // Attempt to get the list of countries from the cache
            List<Country> countries = Cache.Get<List<Country>>(CacheKey);
            if (countries == null)
            {
                // Not cahced, so retrieve them from the service and cache them
                string route = @$"{Settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/1/{AllCountriesPageSize}";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                countries = JsonConvert.DeserializeObject<List<Country>>(json, JsonSettings).OrderBy(m => m.Name).ToList();
                Cache.Set(CacheKey, countries, Settings.Value.CacheLifetimeSeconds);
            }

            // Extract and return the page of interest
            var page = countries.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return page;
        }

        /// <summary>
        /// Return the country with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Country> GetCountryAsync(int id)
        {
            Country country = Cache.Get<List<Country>>(CacheKey)?.FirstOrDefault(l => l.Id == id);
            if (country == null)
            {
                string route = @$"{Settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                country = JsonConvert.DeserializeObject<Country>(json, JsonSettings);
            }

            return country;
        }

        /// <summary>
        /// Create a new country
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Country> AddCountryAsync(string name)
        {
            Cache.Remove(CacheKey);
            string data = $"\"{name}\"";
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            Country country = JsonConvert.DeserializeObject<Country>(json, JsonSettings);
            return country;
        }

        /// <summary>
        /// Update an existing country
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Country> UpdateCountryAsync(int id, string name)
        {
            Cache.Remove(CacheKey);
            string data = $"\"{name}\"";
            string route = @$"{Settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
            string json = await SendDirectAsync(route, data, HttpMethod.Put);
            Country country = JsonConvert.DeserializeObject<Country>(json, JsonSettings);
            return country;
        }
    }
}
