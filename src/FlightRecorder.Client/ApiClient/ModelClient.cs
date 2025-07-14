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
    public class ModelClient : FlightRecorderClientBase, IModelClient
    {
        private const string RouteKey = "Models";
        private const string CacheKeyPrefix = "Models";
        private const int AllModelsPageSize = 1000000;

        private IManufacturerClient _manufacturers;

        public ModelClient(
            IFlightRecorderHttpClient client,
            FlightRecorderApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ICacheWrapper cache,
            ILogger<ModelClient> logger,
            IManufacturerClient manufacturers)
            : base(client, settings, tokenProvider, cache, logger)
        {
            _manufacturers = manufacturers;
        }

        /// <summary>
        /// Return a list of aircraft models
        /// </summary>
        /// <param name="manufacturerId"></param>
        /// <returns></returns>
        public async Task<List<Model>> GetModelsAsync(long manufacturerId)
        {
            string key = $"{CacheKeyPrefix}.{manufacturerId}";
            List<Model> models = Cache.Get<List<Model>>(key);
            if (models == null)
            {
                string route = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}/manufacturer/{manufacturerId}/1/{AllModelsPageSize}";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                if (!string.IsNullOrEmpty(json))
                {
                    models = Deserialize<List<Model>>(json)?.OrderBy(m => m.Name).ToList();
                    Cache.Set(key, models, Settings.CacheLifetimeSeconds);
                }
            }

            return models;
        }

        /// <summary>
        /// Return the model with the specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Model> GetModelAsync(long id)
        {
            // See if the model exists in the cached model lists, first
            Model model = FindCachedModelById(id);
            if (model == null)
            {
                // It doesn't, so go to the service to get it
                string route = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
                string json = await SendDirectAsync(route, null, HttpMethod.Get);
                model = Deserialize<Model>(json);
            }

            return model;
        }

        /// <summary>
        /// Create a new model
        /// </summary>
        /// <param name="name"></param>
        /// <param name="manufacturerId"></param>
        /// <returns></returns>
        public async Task<Model> AddModelAsync(string name, long manufacturerId)
        {
            ClearCachedModelsByManufacturer(manufacturerId);

            dynamic template = new
            {
                Name = name,
                ManufacturerId = manufacturerId
            };

            string data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Post);

            Model model = Deserialize<Model>(json);
            return model;
        }

        /// <summary>
        /// Update an existing model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="manufacturerId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Model> UpdateModelAsync(long id, long manufacturerId, string name)
        {
            ClearCachedModelById(id);
            ClearCachedModelsByManufacturer(manufacturerId);

            dynamic template = new
            {
                Id = id,
                Name = name,
                ManufacturerId = manufacturerId
            };

            string data = Serialize(template);
            string json = await SendIndirectAsync(RouteKey, data, HttpMethod.Put);
            Model model = Deserialize<Model>(json);
            return model;
        }

        /// <summary>
        /// Delete an existing manufacturer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task DeleteManufacturerAsync(long id)
        {
            ClearCachedModelById(id);
            string route = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}/{id}/";
            _ = await SendDirectAsync(route, null, HttpMethod.Delete);
        }

        /// <summary>
        /// Locate the model with the specified ID in the cached model lists
        /// </summary>
        /// <returns></returns>
        private Model FindCachedModelById(long id)
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

        /// <summary>
        /// Clear cached model by model ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private void ClearCachedModelById(long id)
        {
            Model model = FindCachedModelById(id);
            if (model != null)
            {
                ClearCachedModelsByManufacturer(model.ManufacturerId);
            }
        }

        /// <summary>
        /// Clear cached models by manufacturer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private void ClearCachedModelsByManufacturer(long manufacturerId)
        {
            string key = $"{CacheKeyPrefix}.{manufacturerId}";
            Cache.Remove(key);
        }
    }
}
