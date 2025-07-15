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
    public class CountriesClient : FlightRecorderClientBase, ICountriesClient
    {
        private const string RouteKey = "Countries";
        private const string CacheKey = "Countries";

        public CountriesClient(
            IFlightRecorderHttpClient client,
            FlightRecorderApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ICacheWrapper cache,
            ILogger<CountriesClient> logger)
            : base(client, settings, tokenProvider, cache, logger)
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
                // Not cached, so retrieve them from the service and cache them
                string route = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}/1/{int.MaxValue}";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                countries = Deserialize<List<Country>>(json)?.OrderBy(m => m.Name).ToList();
                Cache.Set(CacheKey, countries, Settings.CacheLifetimeSeconds);
            }

            // Extract and return the page of interest
            var page = countries?.Skip((pageNumber - 1) * pageSize)?.Take(pageSize)?.ToList();
            return page;
        }

        /// <summary>
        /// Return the country with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Country> GetCountryAsync(long id)
        {
            Country country = Cache.Get<List<Country>>(CacheKey)?.FirstOrDefault(l => l.Id == id);
            if (country == null)
            {
                string route = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                country = Deserialize<Country>(json);
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
            Country country = Deserialize<Country>(json);
            return country;
        }

        /// <summary>
        /// Update an existing country
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Country> UpdateCountryAsync(long id, string name)
        {
            Cache.Remove(CacheKey);
            string data = $"\"{name}\"";
            string route = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
            string json = await SendDirectAsync(route, data, HttpMethod.Put);
            Country country = Deserialize<Country>(json);
            return country;
        }

        /// <summary>
        /// Delete an existing country
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task DeleteCountryAsync(long id)
        {
            Cache.Remove(CacheKey);
            string route = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
            _ = await SendDirectAsync(route, null, HttpMethod.Delete);
        }
    }
}
