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
    public class AirlineClient : FlightRecorderClientBase
    {
        private const string RouteKey = "Airlines";
        private const string CacheKey = "Airlines";
        private const int AllAirlinesPageSize = 1000000;

        public AirlineClient(HttpClient client, IOptions<AppSettings> settings, IHttpContextAccessor accessor, ICacheWrapper cache)
            : base(client, settings, accessor, cache)
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
                string route = @$"{Settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/1/{AllAirlinesPageSize}";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                airlines = JsonConvert.DeserializeObject<List<Airline>>(json, JsonSettings).OrderBy(m => m.Name).ToList();
                Cache.Set(CacheKey, airlines, Settings.Value.CacheLifetimeSeconds);
            }
            return airlines;
        }

        /// <summary>
        /// Return the airline with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Airline> GetAirlineAsync(int id)
        {
            Airline airline = Cache.Get<List<Airline>>(CacheKey)?.FirstOrDefault(l => l.Id == id);
            if (airline == null)
            {
                string route = @$"{Settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                airline = JsonConvert.DeserializeObject<Airline>(json, JsonSettings);
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
            Airline airline = JsonConvert.DeserializeObject<Airline>(json, JsonSettings);
            return airline;
        }

        /// <summary>
        /// Update an existing airline
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Airline> UpdateAirlineAsync(int id, string name)
        {
            Cache.Remove(CacheKey);
            string data = $"\"{name}\"";
            string route = @$"{Settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
            string json = await SendDirectAsync(route, data, HttpMethod.Put);
            Airline airline = JsonConvert.DeserializeObject<Airline>(json, JsonSettings);
            return airline;
        }
    }
}
