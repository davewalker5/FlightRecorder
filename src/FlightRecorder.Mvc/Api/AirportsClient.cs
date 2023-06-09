using System;
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
    public class AirportsClient : FlightRecorderClientBase
    {
        private const string RouteKey = "Airports";
        private const string CacheKeyPrefix = "Airports";
        private const int AllAirportsPageSize = 1000000;

        private CountriesClient _countries;

        public AirportsClient(HttpClient client, IOptions<AppSettings> settings, IHttpContextAccessor accessor, ICacheWrapper cache, CountriesClient countries)
            : base(client, settings, accessor, cache)
        {
            _countries = countries;
        }

        /// <summary>
        /// Return a list of airports
        /// </summary>
        /// <returns></returns>
        public async Task<List<Airport>> GetAirportsAsync()
        {
            List<Airport> airports = _cache.Get<List<Airport>>(CacheKeyPrefix);
            if (airports == null)
            {
                string route = @$"{_settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/1/{AllAirportsPageSize}";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                airports = JsonConvert.DeserializeObject<List<Airport>>(json).OrderBy(m => m.Name).OrderBy(m => m.Code).ToList();
                _cache.Set(CacheKeyPrefix, airports, _settings.Value.CacheLifetimeSeconds);
            }
            return airports;
        }

        /// <summary>
        /// Return a list of airports with the specified code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<List<Airport>> GetAirportsByCodeAsync(string code)
        {
            string key = $"{CacheKeyPrefix}.R.{code}";
            List<Airport> airports = _cache.Get<List<Airport>>(key);
            if (airports == null)
            {
                string route = @$"{_settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/code/{code}/1/{AllAirportsPageSize}";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                if (!string.IsNullOrEmpty(json))
                {
                    airports = JsonConvert.DeserializeObject<List<Airport>>(json)
                                        .OrderBy(m => m.Code)
                                        .ToList();
                    _cache.Set(key, airports, _settings.Value.CacheLifetimeSeconds);
                }
            }

            return airports;
        }

        /// <summary>
        /// Return the airport with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Airport> GetAirportByIdAsync(int id)
        {
            // See if the flight exists in the cached flight lists, first
            Airport airport = FindCachedAirport(a => a.Id == id);
            if (airport == null)
            {
                // It doesn't, so go to the service to get it
                string route = @$"{_settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                airport = JsonConvert.DeserializeObject<Airport>(json);
            }

            return airport;
        }

        /// <summary>
        /// Add a new airport
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public async Task<Airport> AddAirportAsync(string code, string name, int countryId)
        {
            // Adding an airport changes cached lists of airports, so clear them
            ClearCache();

            // TODO : When the service "TODO" list is completed, it will no longer
            // be necessary to retrieve the country here as it will be possible
            // to pass the country ID in the template
            Country country = await _countries.GetCountryAsync(countryId);

            dynamic template = new
            {
                Code = code,
                Name = name,
                Country = country
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);

            Airport airport = JsonConvert.DeserializeObject<Airport>(json);
            return airport;
        }

        /// <summary>
        /// Update an existing airport
        /// </summary>
        /// <param name="airportId"></param>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public async Task<Airport> UpdateAirportAsync(int airportId, string code, string name, int countryId)
        {
            // Updating an airport changes cached lists of airports, so clear them
            ClearCache();

            dynamic template = new
            {
                Id = airportId,
                Code = code,
                Name = name,
                CountryId = countryId
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);

            Airport airport = JsonConvert.DeserializeObject<Airport>(json);
            return airport;
        }

        /// <summary>
        /// Locate the airport matching the specified expression in the cached lists
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        private Airport FindCachedAirport(Func<Airport, bool> predicate)
        {
            Airport airport = null;

            IEnumerable<string> keys = _cache.GetKeys().Where(k => k.StartsWith(CacheKeyPrefix));
            foreach (string key in keys)
            {
                List<Airport> airports = _cache.Get<List<Airport>>(key);
                airport = airports.FirstOrDefault(predicate);
                if (airport != null)
                {
                    break;
                }
            }

            return airport;
        }

        /// <summary>
        /// Clear all cached airport information
        /// </summary>
        private void ClearCache()
        {
            IEnumerable<string> keys = _cache.GetKeys().Where(k => k.StartsWith(CacheKeyPrefix));
            foreach (string key in keys)
            {
                _cache.Remove(key);
            }
        }
    }
}
