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
    public class AircraftClient : FlightRecorderClientBase, IAircraftClient
    {
        private const string RouteKey = "Aircraft";
        private const string CacheKeyPrefix = "Aircraft";
        private const int AllAircraftPageSize = 1000000;

        private readonly IModelClient _models;

        public AircraftClient(
            IFlightRecorderHttpClient client,
            FlightRecorderApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ICacheWrapper cache,
            ILogger<AircraftClient> logger,
            IModelClient models)
            : base(client, settings, tokenProvider, cache, logger)
        {
            _models = models;
        }

        /// <summary>
        /// Return a list of aircraft of the specified model
        /// </summary>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public async Task<List<Aircraft>> GetAircraftByModelAsync(long modelId)
        {
            string key = $"{CacheKeyPrefix}.{modelId}";
            List<Aircraft> aircraft = Cache.Get<List<Aircraft>>(key);
            if (aircraft == null)
            {
                string route = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}/model/{modelId}/1/{AllAircraftPageSize}";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                if (!string.IsNullOrEmpty(json))
                {
                    aircraft = Deserialize<List<Aircraft>>(json)
                                        ?.OrderBy(m => m.Registration)
                                        .ToList();

                    foreach (Aircraft aeroplane in aircraft.Where(a => a.Manufactured == 0))
                    {
                        aeroplane.Manufactured = null;
                    }

                    Cache.Set(key, aircraft, Settings.CacheLifetimeSeconds);
                }
            }

            return aircraft;
        }

        /// <summary>
        /// Return the aircraft with the specified registration number
        /// </summary>
        /// <param name="registration"></param>
        /// <returns></returns>
        public async Task<Aircraft> GetAircraftByRegistrationAsync(string registration)
        {
            // See if the aircraft exists in the cached aircraft lists, first
            Aircraft aircraft = FindCachedAircraft(a => a.Registration == registration);
            if (aircraft == null)
            {
                // It doesn't, so go to the service to get it
                string route = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}/registration/{registration}/";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                aircraft = Deserialize<Aircraft>(json);
            }

            if ((aircraft != null) && (aircraft.Manufactured == 0))
            {
                aircraft.Manufactured = null;
            }

            return aircraft;
        }

        /// <summary>
        /// Return the aircraft with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Aircraft> GetAircraftByIdAsync(long id)
        {
            // See if the aircraft exists in the cached aircraft lists, first
            Aircraft aircraft = FindCachedAircraft(a => a.Id == id);
            if (aircraft == null)
            {
                // It doesn't, so go to the service to get it
                string route = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                aircraft = Deserialize<Aircraft>(json);
            }

            if ((aircraft != null) && (aircraft.Manufactured == 0))
            {
                aircraft.Manufactured = null;
            }

            return aircraft;
        }

        /// <summary>
        /// Create a new aircraft
        /// </summary>
        /// <param name="registration"></param>
        /// <param name="serialNumber"></param>
        /// <param name="yearOfManufacture"></param>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public async Task<Aircraft> AddAircraftAsync(string registration, string serialNumber, long? yearOfManufacture, long? modelId)
        {
            string key = $"{CacheKeyPrefix}.{modelId}";
            Cache.Remove(key);

            dynamic template = new
            {
                Registration = registration,
                SerialNumber = serialNumber ?? "",
                Manufactured = (yearOfManufacture != null) ? yearOfManufacture : 0,
                ModelId = modelId
            };

            string data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);

            Aircraft aircraft = Deserialize<Aircraft>(json);
            if ((aircraft != null) && (aircraft.Manufactured == 0))
            {
                aircraft.Manufactured = null;
            }

            return aircraft;
        }

        /// <summary>
        /// Update an existing aircraft
        /// </summary>
        /// <param name="id"></param>
        /// <param name="registration"></param>
        /// <param name="serialNumber"></param>
        /// <param name="yearOfManufacture"></param>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public async Task<Aircraft> UpdateAircraftAsync(long id, string registration, string serialNumber, int? yearOfManufacture, long modelId)
        {
            // We might've changed the model, so not only do we need to clear the
            // current model's cached model list but we also need to identify the
            // original model and clear the cached list of aircraft for that, too
            string key;
            Aircraft original = FindCachedAircraft(async => async.Id == id);
            if (original != null)
            {
                key = $"{CacheKeyPrefix}.{original.ModelId}";
                Cache.Remove(key);
            }

            key = $"{CacheKeyPrefix}.{modelId}";
            Cache.Remove(key);

            dynamic template = new
            {
                Id = id,
                ModelId = modelId,
                Registration = registration,
                SerialNumber = serialNumber ?? "",
                Manufactured = (yearOfManufacture != null) ? yearOfManufacture : 0
            };

            string data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);

            Aircraft aircraft = Deserialize<Aircraft>(json);
            if ((aircraft != null) && (aircraft.Manufactured == 0))
            {
                aircraft.Manufactured = null;
            }

            return aircraft;
        }

        /// <summary>
        /// Delete an existing aircraft
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task DeleteAircraftAsync(long id)
        {
            // If the aircraft's been cached, clear it from the cache
            string key;
            Aircraft aircraft = FindCachedAircraft(async => async.Id == id);
            if (aircraft != null)
            {
                key = $"{CacheKeyPrefix}.{aircraft.ModelId}";
                Cache.Remove(key);
            }

            // Delete the aircraft
            string route = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
            _ = await SendDirectAsync(route, null, HttpMethod.Delete);
        }

        /// <summary>
        /// Locate the aircraft matching the specified expression in the cached lists
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        private Aircraft FindCachedAircraft(Func<Aircraft, bool> predicate)
        {
            Aircraft aircraft = null;

            IEnumerable<string> keys = Cache.GetKeys().Where(k => k.StartsWith(CacheKeyPrefix));
            foreach (string key in keys)
            {
                List<Aircraft> aircraftList = Cache.Get<List<Aircraft>>(key);
                aircraft = aircraftList.FirstOrDefault(predicate);
                if (aircraft != null)
                {
                    break;
                }
            }

            return aircraft;
        }
    }
}
