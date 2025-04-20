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
    public class ModelClient : FlightRecorderClientBase
    {
        private const string RouteKey = "Models";
        private const string CacheKeyPrefix = "Models";
        private const int AllModelsPageSize = 1000000;

        private ManufacturerClient _manufacturers;

        public ModelClient(HttpClient client, IOptions<AppSettings> settings, IHttpContextAccessor accessor, ICacheWrapper cache, ManufacturerClient manufacturers)
            : base(client, settings, accessor, cache)
        {
            _manufacturers = manufacturers;
        }

        /// <summary>
        /// Return a list of aircraft models
        /// </summary>
        /// <param name="manufacturerId"></param>
        /// <returns></returns>
        public async Task<List<Model>> GetModelsAsync(int manufacturerId)
        {
            string key = $"{CacheKeyPrefix}.{manufacturerId}";
            List<Model> models = Cache.Get<List<Model>>(key);
            if (models == null)
            {
                string route = @$"{Settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/manufacturer/{manufacturerId}/1/{AllModelsPageSize}";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                if (!string.IsNullOrEmpty(json))
                {
                    models = JsonConvert.DeserializeObject<List<Model>>(json, JsonSettings)
                                        .OrderBy(m => m.Name).ToList();
                    Cache.Set(key, models, Settings.Value.CacheLifetimeSeconds);
                }
            }

            return models;
        }

        /// <summary>
        /// Return the model with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Model> GetModelAsync(int id)
        {
            // See if the model exists in the cached model lists, first
            Model model = FindCachedModelById(id);
            if (model == null)
            {
                // It doesn't, so go to the service to get it
                string route = @$"{Settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                model = JsonConvert.DeserializeObject<Model>(json, JsonSettings);
            }

            return model;
        }

        /// <summary>
        /// Create a new model
        /// </summary>
        /// <param name="name"></param>
        /// <param name="manufacturerId"></param>
        /// <returns></returns>
        public async Task<Model> AddModelAsync(string name, int manufacturerId)
        {
            string key = $"{CacheKeyPrefix}.{manufacturerId}";
            Cache.Remove(key);

            // TODO : When the service "TODO" list is completed, it will no longer
            // be necessary to retrieve the manufacturer here as it will be
            // possible to pass the manufacturer ID in the template
            Manufacturer manufacturer = await _manufacturers.GetManufacturerAsync(manufacturerId);

            dynamic template = new
            {
                Name = name,
                Manufacturer = manufacturer
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);

            Model model = JsonConvert.DeserializeObject<Model>(json, JsonSettings);
            return model;
        }

        /// <summary>
        /// Update an existing model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="manufacturerId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Model> UpdateModelAsync(int id, int manufacturerId, string name)
        {
            // We might've changed the manufacturer, so not only do we need to clear the
            // current manufacturer's cached model list but we also need to identify the
            // original manufacturer and clear the cached list of models for that, too
            string key;
            Model original = FindCachedModelById(id);
            if (original != null)
            {
                key = $"{CacheKeyPrefix}.{original.ManufacturerId}";
                Cache.Remove(key);
            }

            key = $"{CacheKeyPrefix}.{manufacturerId}";
            Cache.Remove(key);

            dynamic template = new
            {
                Id = id,
                Name = name,
                ManufacturerId = manufacturerId
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            Model model = JsonConvert.DeserializeObject<Model>(json, JsonSettings);
            return model;
        }

        /// <summary>
        /// Locate the model with the specified ID in the cached model lists
        /// </summary>
        /// <returns></returns>
        private Model FindCachedModelById(int id)
        {
            Model model = null;

            IEnumerable<string> keys = Cache.GetKeys().Where(k => k.StartsWith(CacheKeyPrefix));
            foreach (string key in keys)
            {
                List<Model> models = Cache.Get<List<Model>>(key);
                model = models.FirstOrDefault(m => m.Id == id);
            }

            return model;
        }
    }
}
