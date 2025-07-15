using System;
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
    public class FlightClient : FlightRecorderClientBase, IFlightClient
    {
        private const string RouteKey = "Flights";
        private const string CacheKeyPrefix = "Flights";

        private IAirlineClient _airlines;

        public FlightClient(
            IFlightRecorderHttpClient client,
            FlightRecorderApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ICacheWrapper cache,
            ILogger<FlightClient> logger,
            IAirlineClient airlines)
            : base(client, settings, tokenProvider, cache, logger)
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
            List<Flight> flights = Cache.Get<List<Flight>>(key);
            if (flights == null)
            {
                string route = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}/route/{embarkation}/{destination}/1/{int.MaxValue}";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                if (!string.IsNullOrEmpty(json))
                {
                    flights = Deserialize<List<Flight>>(json)
                                        ?.OrderBy(m => m.Embarkation)
                                        .ThenBy(m => m.Destination)
                                        .ToList();
                    Cache.Set(key, flights, Settings.CacheLifetimeSeconds);
                }
            }

            return flights;
        }

        /// <summary>
        /// Return a list of flights for the specified airline
        /// </summary>
        /// <param name="airlineId"></param>
        /// <returns></returns>
        public async Task<List<Flight>> GetFlightsByAirlineAsync(long airlineId)
        {
            string key = $"{CacheKeyPrefix}.A.{airlineId}";
            List<Flight> flights = Cache.Get<List<Flight>>(key);
            if (flights == null)
            {
                string route = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}/airline/{airlineId}/1/{int.MaxValue}";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                if (!string.IsNullOrEmpty(json))
                {
                    flights = Deserialize<List<Flight>>(json)
                                        ?.OrderBy(m => m.Airline.Name)
                                        .ThenBy(m => m.Embarkation)
                                        .ThenBy(m => m.Destination)
                                        .ToList();
                    Cache.Set(key, flights, Settings.CacheLifetimeSeconds);
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
            List<Flight> flights = Cache.Get<List<Flight>>(key);
            if (flights == null)
            {
                string route = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}/number/{number}/1/{int.MaxValue}";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                if (!string.IsNullOrEmpty(json))
                {
                    flights = Deserialize<List<Flight>>(json)
                                        ?.OrderBy(m => m.Airline.Name)
                                        .ThenBy(m => m.Embarkation)
                                        .ThenBy(m => m.Destination)
                                        .ToList();
                    Cache.Set(key, flights, Settings.CacheLifetimeSeconds);
                }
            }

            return flights;
        }

        /// <summary>
        /// Return the flight with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Flight> GetFlightByIdAsync(long id)
        {
            // See if the flight exists in the cached flight lists, first
            Flight flight = FindCachedFlight(a => a.Id == id);
            if (flight == null)
            {
                // It doesn't, so go to the service to get it
                string route = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                flight = Deserialize<Flight>(json);
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
        public async Task<Flight> AddFlightAsync(string number, string embarkation, string destination, long airlineId)
        {
            // Adding a flight changes cached lists of flights by number, route and
            // airline - just clear the lot!
            ClearCache();

            dynamic template = new
            {
                Number = number,
                Embarkation = embarkation,
                Destination = destination,
                AirlineId = airlineId
            };

            string data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);

            Flight flight = Deserialize<Flight>(json);
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
        public async Task<Flight> UpdateFlightAsync(long flightId, string number, string embarkation, string destination, long airlineId)
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

            string data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);

            Flight flight = Deserialize<Flight>(json);
            return flight;
        }

        /// <summary>
        /// Delete an existing flight
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task DeleteFlightAsync(long id)
        {
            ClearCache();
            string route = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
            _ = await SendDirectAsync(route, null, HttpMethod.Delete);
        }

        /// <summary>
        /// Locate the flight matching the specified expression in the cached lists
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        private Flight FindCachedFlight(Func<Flight, bool> predicate)
        {
            Flight flight = null;

            IEnumerable<string> keys = Cache.GetKeys().Where(k => k.StartsWith(CacheKeyPrefix));
            foreach (string key in keys)
            {
                List<Flight> flights = Cache.Get<List<Flight>>(key);
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
            IEnumerable<string> keys = Cache.GetKeys().Where(k => k.StartsWith(CacheKeyPrefix));
            foreach (string key in keys)
            {
                Cache.Remove(key);
            }
        }
    }
}
