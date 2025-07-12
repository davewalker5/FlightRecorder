using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FlightRecorder.Client.Interfaces;
using Microsoft.Extensions.Logging;

namespace FlightRecorder.Client.ApiClient
{
    [ExcludeFromCodeCoverage]
    public sealed class FlightRecorderHttpClient : IFlightRecorderHttpClient
    {
        private readonly static HttpClient _client = new();
        private static FlightRecorderHttpClient _instance = null;
        private readonly static object _lock = new();

        private FlightRecorderHttpClient() { }

        /// <summary>
        /// Return the singleton instance of the client
        /// </summary>
        public static FlightRecorderHttpClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new FlightRecorderHttpClient();
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Send a GET request to the specified URI and return the response
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="token"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAsync(string uri, string token, ILogger logger)
            => await SendRequestAsync(HttpMethod.Get, null, uri, token, logger);

        /// <summary>
        /// POST data to the specified URI
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="content"></param>
        /// <param name="token"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostAsync(string uri, HttpContent content, string token, ILogger logger)
            => await SendRequestAsync(HttpMethod.Post, content, uri, token, logger);

        /// <summary>
        /// Send a PUT request to update the resource at the specified URI
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="content"></param>
        /// <param name="token"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PutAsync(string uri, HttpContent content, string token, ILogger logger)
            => await SendRequestAsync(HttpMethod.Put, content, uri, token, logger);

        /// <summary>
        /// Send a DELETE request to delete the resource at the specified URI
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DeleteAsync(string uri, string token, ILogger logger)
            => await SendRequestAsync(HttpMethod.Delete, null, uri, token, logger);

        /// <summary>
        /// Send a request to the API, with logging
        /// </summary>
        /// <param name="method"></param>
        /// <param name="content"></param>
        /// <param name="uri"></param>
        /// <param name="token"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> SendRequestAsync(HttpMethod method, HttpContent content, string uri, string token, ILogger logger)
        {
            logger.LogDebug($"Sending {method} request to {uri}");
            logger.LogDebug($"API token = {token?[..50]}");

            // Create the request object
            var request = new HttpRequestMessage(method, uri);

            // Set the authorisation header if we have a potentially valid API token
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // Set the request payload
            if (content != null)
            {
                logger.LogDebug($"Setting request content");
                request.Content = content;
            }

            // Log the request headers
            foreach (var header in request.Headers)
            {
                LogHeaderValues("Request", header, logger);
            }

            // Send the request and retrieve the response
            var response = await _client.SendAsync(request);
            logger.LogDebug($"Response status code = {response.StatusCode}");

            // Log the response headers
            foreach (var header in response.Headers)
            {
                LogHeaderValues("Response", header, logger);
            }

            return response;
        }

        /// <summary>
        /// Log the values for a single header
        /// </summary>
        /// <param name="type"></param>
        /// <param name="request"></param>
        /// <param name="logger"></param>
        private void LogHeaderValues(string type, KeyValuePair<string, IEnumerable<string>> header, ILogger logger)
        {
            foreach (var value in header.Value)
            {
                logger.LogDebug($"{type} Header {header.Key} = {value}");
            }
        }
    }
}