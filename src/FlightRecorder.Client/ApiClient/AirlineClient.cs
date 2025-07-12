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
    public class AirlineClient : FlightRecorderClientBase, IAirlineClient
    {
        private const string RouteKey = "Airlines";
        private const string CacheKey = "Airlines";
        private const int AllAirlinesPageSize = 1000000;

        public AirlineClient(
            IFlightRecorderHttpClient client,
            FlightRecorderApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ICacheWrapper cache,
            ILogger<AirlineClient> logger)
            : base(client, settings, tokenProvider, cache, logger)
        {
        }

        /// <summary>
        /// Return a list of airlines
        /// </summary>
        /// <returns></returns>
        public async Task<List<Airline>> GetAirlinesAsync()
        {
            List<Airline> airlines = Cache.Get<List<Airline>>(CacheKey);
            if (airlines == null)
            {
                string route = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}/1/{AllAirlinesPageSize}";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                airlines = Deserialize<List<Airline>>(json)?.OrderBy(m => m.Name).ToList();
                Cache.Set(CacheKey, airlines, Settings.CacheLifetimeSeconds);
            }
            return airlines;
        }

        /// <summary>
        /// Return the airline with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Airline> GetAirlineAsync(long id)
        {
            Airline airline = Cache.Get<List<Airline>>(CacheKey)?.FirstOrDefault(l => l.Id == id);
            if (airline == null)
            {
                string route = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                airline = Deserialize<Airline>(json);
            }

            return airline;
        }

        /// <summary>
        /// Create a new airline
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Airline> AddAirlineAsync(string name)
        {
            Cache.Remove(CacheKey);
            string data = $"\"{name}\"";
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);
            Airline airline = Deserialize<Airline>(json);
            return airline;
        }

        /// <summary>
        /// Update an existing airline
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Airline> UpdateAirlineAsync(long id, string name)
        {
            Cache.Remove(CacheKey);
            string data = $"\"{name}\"";
            string route = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
            string json = await SendDirectAsync(route, data, HttpMethod.Put);
            Airline airline = Deserialize<Airline>(json);
            return airline;
        }
    }
}
