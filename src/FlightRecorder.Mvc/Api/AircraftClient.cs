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
    public class AircraftClient : FlightRecorderClientBase
    {
        private const string RouteKey = "Aircraft";
        private const string CacheKeyPrefix = "Aircraft";
        private const int AllAircraftPageSize = 1000000;

        private ModelClient _models;

        public AircraftClient(HttpClient client, IOptions<AppSettings> settings, IHttpContextAccessor accessor, ICacheWrapper cache, ModelClient models)
            : base(client, settings, accessor, cache)
        {
            _models = models;
        }

        /// <summary>
        /// Return a list of aircraft of the specified model
        /// </summary>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public async Task<List<Aircraft>> GetAircraftByModelAsync(int modelId)
        {
            string key = $"{CacheKeyPrefix}.{modelId}";
            List<Aircraft> aircraft = _cache.Get<List<Aircraft>>(key);
            if (aircraft == null)
            {
                string route = @$"{_settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/model/{modelId}/1/{AllAircraftPageSize}";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                if (!string.IsNullOrEmpty(json))
                {
                    aircraft = JsonConvert.DeserializeObject<List<Aircraft>>(json)
                                        .OrderBy(m => m.Registration)
                                        .ToList();
                    _cache.Set(key, aircraft, _settings.Value.CacheLifetimeSeconds);
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
                string route = @$"{_settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/registration/{registration}/";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                if (!string.IsNullOrEmpty(json))
                {
                    aircraft = JsonConvert.DeserializeObject<Aircraft>(json);
                }
            }

            return aircraft;
        }

        /// <summary>
        /// Return the aircraft with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Aircraft> GetAircraftByIdAsync(int id)
        {
            // See if the aircraft exists in the cached aircraft lists, first
            Aircraft aircraft = FindCachedAircraft(a => a.Id == id);
            if (aircraft == null)
            {
                // It doesn't, so go to the service to get it
                string route = @$"{_settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                aircraft = JsonConvert.DeserializeObject<Aircraft>(json);
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
        public async Task<Aircraft> AddAircraftAsync(string registration, string serialNumber, int yearOfManufacture, int modelId)
        {
            string key = $"{CacheKeyPrefix}.{modelId}";
            _cache.Remove(key);

            // TODO : When the service "TODO" list is completed, it will no longer
            // be necessary to retrieve the model here as it will be possible
            // to pass the model ID in the template
            Model model = await _models.GetModelAsync(modelId);

            dynamic template = new
            {
                Registration = registration,
                SerialNumber = serialNumber,
                Manufactured = yearOfManufacture,
                Model = model
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);

            Aircraft aircraft = JsonConvert.DeserializeObject<Aircraft>(json);
            return aircraft;
        }

        /// <summary>
        /// Update an existing aircraft
        /// </summary>
        /// <param name="id"></param>
        /// <param name="registration"></param>
        /// <param name="serialNumber"></param>
        /// <param name="yearOfManufacture"></param>
        /// <param name="manufacturerId"></param>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public async Task<Aircraft> UpdateAircraftAsync(int id, string registration, string serialNumber, int yearOfManufacture, int manufacturerId, int modelId)
        {
            // We might've changed the model, so not only do we need to clear the
            // current model's cached model list but we also need to identify the
            // original model and clear the cached list of aircraft for that, too
            string key;
            Aircraft original = FindCachedAircraft(async => async.Id == id);
            if (original != null)
            {
                key = $"{CacheKeyPrefix}.{original.ModelId}";
                _cache.Remove(key);
            }

            key = $"{CacheKeyPrefix}.{modelId}";
            _cache.Remove(key);

            dynamic template = new
            {
                Id = id,
                ModelId = modelId,
                Registration = registration,
                SerialNumber = serialNumber,
                Manufactured = yearOfManufacture,
                Model = new Model
                {
                    ManufacturerId = manufacturerId
                }
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);

            Aircraft aircraft = JsonConvert.DeserializeObject<Aircraft>(json);
            return aircraft;
        }

        /// <summary>
        /// Locate the aircraft matching the specified expression in the cached lists
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        private Aircraft FindCachedAircraft(Func<Aircraft, bool> predicate)
        {
            Aircraft aircraft = null;

            IEnumerable<string> keys = _cache.GetKeys().Where(k => k.StartsWith(CacheKeyPrefix));
            foreach (string key in keys)
            {
                List<Aircraft> aircraftList = _cache.Get<List<Aircraft>>(key);
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
