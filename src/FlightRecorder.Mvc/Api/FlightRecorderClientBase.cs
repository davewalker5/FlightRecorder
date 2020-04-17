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

namespace FlightRecorder.Mvc.Api
{
    public abstract class FlightRecorderClientBase
    {
        protected HttpClient _client { get; private set; }
        protected IOptions<AppSettings> _settings { get; private set; }
        protected ICacheWrapper _cache { get; private set; }

        public FlightRecorderClientBase(HttpClient client, IOptions<AppSettings> settings, IHttpContextAccessor accessor, ICacheWrapper cache)
        {
            _cache = cache;

            // Configure the Http client
            _settings = settings;
            _client = client;
            _client.BaseAddress = new Uri(_settings.Value.ApiUrl);

            // Retrieve the token from session and create the authentication
            // header, if present
            string token = accessor.HttpContext.Session.GetString(LoginController.TokenSessionKey);
            if (!string.IsNullOrEmpty(token))
            {
                SetAuthenticationHeader(token);
            }
        }

        /// <summary>
        /// Add the authorization header to the default request headers
        /// </summary>
        /// <param name="token"></param>
        protected void SetAuthenticationHeader(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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
            string route = _settings.Value.ApiRoutes.First(r => r.Name == routeName).Route;
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
                response = await _client.GetAsync(route);
            }
            else if (method == HttpMethod.Post)
            {
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                response = await _client.PostAsync(route, content);
            }
            else if (method == HttpMethod.Put)
            {
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                response = await _client.PutAsync(route, content);
            }

            if ((response != null) && response.IsSuccessStatusCode)
            {
                json = await response.Content.ReadAsStringAsync();
            }

            return json;
        }
    }
}
