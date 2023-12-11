using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FlightRecorder.Mvc.Configuration;
using FlightRecorder.Mvc.Controllers;
using FlightRecorder.Mvc.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FlightRecorder.Mvc.Api
{
    public abstract class FlightRecorderClientBase
    {
        protected HttpClient Client { get; private set; }
        protected IOptions<AppSettings> Settings { get; private set; }
        protected ICacheWrapper Cache { get; private set; }
        protected JsonSerializerSettings JsonSettings { get; private set; }

        public FlightRecorderClientBase(HttpClient client, IOptions<AppSettings> settings, IHttpContextAccessor accessor, ICacheWrapper cache)
        {
            Cache = cache;

            // Configure the Http client
            Settings = settings;
            Client = client;
            Client.BaseAddress = new Uri(Settings.Value.ApiUrl);

            // Retrieve the token from session and create the authentication
            // header, if present
            string token = accessor.HttpContext.Session.GetString(LoginController.TokenSessionKey);
            if (!string.IsNullOrEmpty(token))
            {
                SetAuthenticationHeader(token);
            }

            // Create the JSON deserialisation settings
            JsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
        }

        /// <summary>
        /// Add the authorization header to the default request headers
        /// </summary>
        /// <param name="token"></param>
        protected void SetAuthenticationHeader(string token)
        {
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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
            string route = Settings.Value.ApiRoutes.First(r => r.Name == routeName).Route;
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

            HttpResponseMessage response = null;
            if (method == HttpMethod.Get)
            {
                response = await Client.GetAsync(route);
            }
            else if (method == HttpMethod.Post)
            {
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                response = await Client.PostAsync(route, content);
            }
            else if (method == HttpMethod.Put)
            {
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                response = await Client.PutAsync(route, content);
            }

            if ((response != null) && response.IsSuccessStatusCode)
            {
                json = await response.Content.ReadAsStringAsync();
            }

            return json;
        }
    }
}
