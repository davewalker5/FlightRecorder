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
    public class FlightClient : FlightRecorderClientBase
    {
        private const string RouteKey = "Flights";
        private const string CacheKeyPrefix = "Flights";
        private const int AllFlightsPageSize = 1000000;

        private AirlineClient _airlines;

        public FlightClient(HttpClient client, IOptions<AppSettings> settings, IHttpContextAccessor accessor, ICacheWrapper cache, AirlineClient airlines)
            : base(client, settings, accessor, cache)
        {
            _airlines = airlines;
        }

        /// <summary>
        /// Return a list of flights for the specified route
        /// </summary>
        /// <param name="embarkation"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public async Task<List<Flight>> GetFlightsByRouteAsync(string embarkation, string destination)
        {
            string key = $"{CacheKeyPrefix}.R.{embarkation}.{destination}";
            List<Flight> flights = _cache.Get<List<Flight>>(key);
            if (flights == null)
            {
                string route = @$"{_settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/route/{embarkation}/{destination}/1/{AllFlightsPageSize}";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                if (!string.IsNullOrEmpty(json))
                {
                    flights = JsonConvert.DeserializeObject<List<Flight>>(json)
                                        .OrderBy(m => m.Embarkation)
                                        .ThenBy(m => m.Destination)
                                        .ToList();
                    _cache.Set(key, flights, _settings.Value.CacheLifetimeSeconds);
                }
            }

            return flights;
        }

        /// <summary>
        /// Return a list of flights for the specified airline
        /// </summary>
        /// <param name="airlineId"></param>
        /// <returns></returns>
        public async Task<List<Flight>> GetFlightsByAirlineAsync(int airlineId)
        {
            string key = $"{CacheKeyPrefix}.A.{airlineId}";
            List<Flight> flights = _cache.Get<List<Flight>>(key);
            if (flights == null)
            {
                string route = @$"{_settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/airline/{airlineId}/1/{AllFlightsPageSize}";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                if (!string.IsNullOrEmpty(json))
                {
                    flights = JsonConvert.DeserializeObject<List<Flight>>(json)
                                        .OrderBy(m => m.Airline.Name)
                                        .ThenBy(m => m.Embarkation)
                                        .ThenBy(m => m.Destination)
                                        .ToList();
                    _cache.Set(key, flights, _settings.Value.CacheLifetimeSeconds);
                }
            }

            return flights;
        }

        /// <summary>
        /// Return a list of flights with the specified number
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public async Task<List<Flight>> GetFlightsByNumberAsync(string number)
        {
            string key = $"{CacheKeyPrefix}.N.{number}";
            List<Flight> flights = _cache.Get<List<Flight>>(key);
            if (flights == null)
            {
                string route = @$"{_settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/number/{number}/1/{AllFlightsPageSize}";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                if (!string.IsNullOrEmpty(json))
                {
                    flights = JsonConvert.DeserializeObject<List<Flight>>(json)
                                        .OrderBy(m => m.Airline.Name)
                                        .ThenBy(m => m.Embarkation)
                                        .ThenBy(m => m.Destination)
                                        .ToList();
                    _cache.Set(key, flights, _settings.Value.CacheLifetimeSeconds);
                }
            }

            return flights;
        }

        /// <summary>
        /// Return the flight with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Flight> GetFlightByIdAsync(int id)
        {
            // See if the flight exists in the cached flight lists, first
            Flight flight = FindCachedFlight(a => a.Id == id);
            if (flight == null)
            {
                // It doesn't, so go to the service to get it
                string route = @$"{_settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                flight = JsonConvert.DeserializeObject<Flight>(json);
            }

            return flight;
        }

        /// <summary>
        /// Add a new flight
        /// </summary>
        /// <param name="number"></param>
        /// <param name="embarkation"></param>
        /// <param name="destination"></param>
        /// <param name="airlineId"></param>
        /// <returns></returns>
        public async Task<Flight> AddFlightAsync(string number, string embarkation, string destination, int airlineId)
        {
            // Adding a flight changes cached lists of flights by number, route and
            // airline - just clear the lot!
            ClearCache();

            // TODO : When the service "TODO" list is completed, it will no longer
            // be necessary to retrieve the airline here as it will be possible
            // to pass the airline ID in the template
            Airline airline = await _airlines.GetAirlineAsync(airlineId);

            dynamic template = new
            {
                Number = number,
                Embarkation = embarkation,
                Destination = destination,
                Airline = airline
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);

            Flight flight = JsonConvert.DeserializeObject<Flight>(json);
            return flight;
        }

        /// <summary>
        /// Update an existing flight
        /// </summary>
        /// <param name="flightId"></param>
        /// <param name="number"></param>
        /// <param name="embarkation"></param>
        /// <param name="destination"></param>
        /// <param name="airlineId"></param>
        /// <returns></returns>
        public async Task<Flight> UpdateFlightAsync(int flightId, string number, string embarkation, string destination, int airlineId)
        {
            // Updating a flight changes cached lists of flights by number, route and
            // airline - just clear the lot!
            ClearCache();

            dynamic template = new
            {
                Id = flightId,
                Number = number,
                Embarkation = embarkation,
                Destination = destination,
                AirlineId = airlineId
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);

            Flight flight = JsonConvert.DeserializeObject<Flight>(json);
            return flight;
        }

        /// <summary>
        /// Locate the flight matching the specified expression in the cached lists
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        private Flight FindCachedFlight(Func<Flight, bool> predicate)
        {
            Flight flight = null;

            IEnumerable<string> keys = _cache.GetKeys().Where(k => k.StartsWith(CacheKeyPrefix));
            foreach (string key in keys)
            {
                List<Flight> flights = _cache.Get<List<Flight>>(key);
                flight = flights.FirstOrDefault(predicate);
                if (flight != null)
                {
                    break;
                }
            }

            return flight;
        }

        /// <summary>
        /// Clear all cached flight information
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
