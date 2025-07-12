using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using FlightRecorder.Entities.Interfaces;

namespace FlightRecorder.BusinessLogic.Api
{
    [ExcludeFromCodeCoverage]
    public class ExternalApiHttpClient : IExternalApiHttpClient
    {
        private readonly static HttpClient _client = new();
#pragma warning disable CS8632
        private static ExternalApiHttpClient? _instance = null;
#pragma warning restore CS8632
        private readonly static object _lock = new();

        private ExternalApiHttpClient() { }

        /// <summary>
        /// Return the singleton instance of the client
        /// </summary>
        public static ExternalApiHttpClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new ExternalApiHttpClient();
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Set the request headers
        /// </summary>
        /// <param name="headers"></param>
        public void SetHeaders(Dictionary<string, string> headers)
        {
            _client.DefaultRequestHeaders.Clear();
            foreach (var header in headers)
            {
                _client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        /// <summary>
        /// Send a GET request to the specified URI and return the response
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAsync(string uri)
            => await _client.GetAsync(uri);
    }
}
