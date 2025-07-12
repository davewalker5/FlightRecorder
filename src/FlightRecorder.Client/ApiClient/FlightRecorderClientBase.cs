using System.Text;
using FlightRecorder.Client.Interfaces;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using FlightRecorder.Entities.Config;
using System.Threading.Tasks;
using System.Net.Http;
using System.Linq;

namespace FlightRecorder.Client.ApiClient
{
    public abstract class FlightRecorderClientBase
    {
        protected IFlightRecorderHttpClient Client { get; private set; }
        protected FlightRecorderApplicationSettings Settings { get; private set; }
        protected IAuthenticationTokenProvider TokenProvider { get; private set; }
        protected ICacheWrapper Cache { get; private set; }
        protected ILogger Logger { get; private set; }

        private readonly JsonSerializerOptions _serializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public FlightRecorderClientBase(
            IFlightRecorderHttpClient client,
            FlightRecorderApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ICacheWrapper cache,
            ILogger logger)
        {
            Client = client;
            Settings = settings;
            TokenProvider = tokenProvider;
            Cache = cache;
            Logger = logger;
        }

        /// <summary>
        /// Given a route name, some data (null in the case of GET) and an HTTP method,
        /// look up the route from the application settings then send the request to
        /// the service and return the JSON response
        /// </summary>
        /// <param name="routeName"></param>
        /// <param name="data"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        protected async Task<string> SendIndirectAsync(string routeName, string data, HttpMethod method)
        {
            string route = Settings.ApiRoutes.First(r => r.Name == routeName).Route;
            string json = await SendDirectAsync(route, data, method);
            return json;
        }

        /// <summary>
        /// Given a route, some data (null in the case of GET) and an HTTP method,
        /// send the request to the service and return the JSON response
        /// </summary>
        /// <param name="route"></param>
        /// <param name="data"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        protected async Task<string> SendDirectAsync(string route, string data, HttpMethod method)
        {
            string json = null;

            var body = !string.IsNullOrEmpty(data) ? data : "No Content";
            Logger.LogDebug($"Sending {method} request to endpoint {route}");
            Logger.LogDebug($"Request body = {body}");

            var token = TokenProvider.GetToken();
            Logger.LogDebug($"API token = {token?[..50]}...");

            HttpResponseMessage response = null;
            if (method == HttpMethod.Get)
            {
                response = await Client.GetAsync($"{Settings.ApiUrl}{route}", token, Logger);
            }
            else if (method == HttpMethod.Post)
            {
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                response = await Client.PostAsync($"{Settings.ApiUrl}{route}", content, token, Logger);
            }
            else if (method == HttpMethod.Put)
            {
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                response = await Client.PutAsync($"{Settings.ApiUrl}{route}", content, token, Logger);
            }
            else if (method == HttpMethod.Delete)
            {
                response = await Client.DeleteAsync($"{Settings.ApiUrl}{route}", token, Logger);
            }

            Logger.LogDebug($"HTTP Status Code = {response?.StatusCode}");
    
            if (response != null)
            {
                // Extract the response content and log it
                json = await response.Content.ReadAsStringAsync();
                var content = json ?? "No Content";
                Logger.LogDebug($"Response content = '{content}'");
            }

            return json;
        }

        /// <summary>
        /// Serialize an object to JSON
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        protected static string Serialize(object o)
            => JsonSerializer.Serialize(o);

        /// <summary>
        /// Deserialize a JSON string to an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        protected T Deserialize<T>(string json) where T : class
            => !string.IsNullOrEmpty(json) ? JsonSerializer.Deserialize<T>(json, _serializerOptions) : null;
    }
}
