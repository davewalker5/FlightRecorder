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
    public class SightingsSearchClient : FlightRecorderClientBase, ISightingsSearchClient
    {
        private const string RouteKey = "Sightings";

        public SightingsSearchClient(
            IFlightRecorderHttpClient client,
            FlightRecorderApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ICacheWrapper cache,
            ILogger<SightingsSearchClient> logger)
            : base(client, settings, tokenProvider, cache, logger)
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
            string baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            string route = $"{baseRoute}/route/{embarkation}/{destination}/{page}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            List<Sighting> sightings = Deserialize<List<Sighting>>(json);
            return (sightings != null) ? sightings.OrderBy(s => s.Date).ToList() : sightings;
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
            string baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            string route = $"{baseRoute}/flight/{number}/{page}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            List<Sighting> sightings = Deserialize<List<Sighting>>(json);
            return (sightings != null) ? sightings.OrderBy(s => s.Date).ToList() : sightings;
        }

        /// <summary>
        /// Get the most recent sighting of a flight
        /// </summary>
        /// <param name="flightNumber"></param>
        /// <returns></returns>
        public async Task<Sighting> GetMostRecentFlightSighting(string flightNumber)
        {
            string baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            string route = $"{baseRoute}/recent/flight/{flightNumber}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            Sighting sighting = Deserialize<Sighting>(json);
            return sighting;
        }

        /// <summary>
        /// Get the most recent sighting of an aircraft
        /// </summary>
        /// <param name="registration"></param>
        /// <returns></returns>
        public async Task<Sighting> GetMostRecentAircraftSighting(string registration)
        {
            string baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            string route = $"{baseRoute}/recent/aircraft/{registration}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            Sighting sighting = Deserialize<Sighting>(json);
            return sighting;
        }

        /// <summary>
        /// Return the specified page of sightings filtered by airline
        /// </summary>
        /// <param name="airlineId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Sighting>> GetSightingsByAirline(long airlineId, int page, int pageSize)
        {
            string baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            string route = $"{baseRoute}/airline/{airlineId}/{page}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            List<Sighting> sightings = Deserialize<List<Sighting>>(json);
            return (sightings != null) ? sightings.OrderBy(s => s.Date).ToList() : sightings;
        }

        /// <summary>
        /// Return the specified page of sightings filtered by aircraft registration
        /// </summary>
        /// <param name="aircraftId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Sighting>> GetSightingsByAircraft(long aircraftId, int page, int pageSize)
        {
            string baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            string route = $"{baseRoute}/aircraft/{aircraftId}/{page}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            List<Sighting> sightings = Deserialize<List<Sighting>>(json);
            return (sightings != null) ? sightings.OrderBy(s => s.Date).ToList() : sightings;
        }

        /// <summary>
        /// Return the specified page of sightings recorded within a given date range
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Sighting>> GetSightingsByDate(DateTime from, DateTime to, int page, int pageSize)
        {
            string fromRouteSegment = from.ToString(Settings.DateTimeFormat);
            string toRouteSegment = to.ToString(Settings.DateTimeFormat);
            string baseRoute = Settings.ApiRoutes.First(r => r.Name == RouteKey).Route;
            string route = $"{baseRoute}/date/{fromRouteSegment}/{toRouteSegment}/{page}/{pageSize}";
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            List<Sighting> sightings = Deserialize<List<Sighting>>(json);
            return (sightings != null) ? sightings.OrderBy(s => s.Date).ToList() : sightings;
        }
    }
}
