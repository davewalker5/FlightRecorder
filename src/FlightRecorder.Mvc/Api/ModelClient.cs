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

        public ModelClient(HttpClient client, IOptions<AppSettings> settings, IHttpContextAccessor accessor, ICacheWrapper cache)
            : base(client, settings, accessor, cache)
        {
        }

        /// <summary>
        /// Return a list of aircraft models
        /// </summary>
        /// <param name="manufacturerId"></param>
        /// <returns></returns>
        public async Task<List<Model>> GetModelsAsync(int manufacturerId)
        {
            string key = $"{CacheKeyPrefix}.{manufacturerId}";
            List<Model> models = _cache.Get<List<Model>>(key);
            if (models == null)
            {
                string route = @$"{_settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/manufacturer/{manufacturerId}/1/{AllModelsPageSize}";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                if (!string.IsNullOrEmpty(json))
                {
                    models = JsonConvert.DeserializeObject<List<Model>>(json)
                                        .OrderBy(m => m.Name).ToList();
                    _cache.Set(key, models, _settings.Value.CacheLifetimeSeconds);
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
            Model model = null;

            // See if the model exists in the cached model lists
            IEnumerable<string> keys = _cache.GetKeys().Where(k => k.StartsWith(CacheKeyPrefix));
            foreach (string key in keys)
            {
                List<Model> models = _cache.Get<List<Model>>(key);
                model = models.FirstOrDefault(m => m.Id == id);
            }

            if (model == null)
            {
                string route = @$"{_settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                model = JsonConvert.DeserializeObject<Model>(json);
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
            _cache.Remove(key);

            dynamic template = new
            {
                Name = name,
                ManufacturerId = manufacturerId
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);

            Model model = JsonConvert.DeserializeObject<Model>(json);
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
            string key = $"{CacheKeyPrefix}.{manufacturerId}";
            _cache.Remove(key);

            dynamic template = new
            {
                Id = id,
                Name = name,
                ManufacturerId = manufacturerId
            };

            string data = JsonConvert.SerializeObject(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            Model model = JsonConvert.DeserializeObject<Model>(json);
            return model;
        }
    }
}
