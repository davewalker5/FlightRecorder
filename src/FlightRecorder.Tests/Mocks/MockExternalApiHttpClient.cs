using FlightRecorder.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlightRecorder.Tests.Mocks
{
    [ExcludeFromCodeCoverage]
    internal class MockExternalApiHttpClient : IExternalApiHttpClient
    {
        private readonly Queue<string> _responses = new();

        /// <summary>
        /// Queue a response
        /// </summary>
        /// <param name="response"></param>
        public void AddResponse(string response)
        {
            _responses.Enqueue(response);
        }

        /// <summary>
        /// Queue a set of responses
        /// </summary>
        /// <param name="responses"></param>
        public void AddResponses(IEnumerable<string> responses)
        {
            foreach (string response in responses)
            {
                _responses.Enqueue(response);
            }
        }

        /// <summary>
        /// Set the HTTP request headers
        /// </summary>
        /// <param name="headers"></param>
        public void SetHeaders(Dictionary<string, string> headers)
        {
        }

        /// <summary>
        /// Construct and return the next response
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
#pragma warning disable CS1998
        public async Task<HttpResponseMessage> GetAsync(string uri)
        {
            // De-queue the next message and throw an exception if it's null
            var content = _responses.Dequeue() ?? throw new Exception();

            // Construct an HTTP response
            var response = new HttpResponseMessage
            {
                Content = new StringContent(content ?? ""),
                StatusCode = HttpStatusCode.OK
            };

            return response;
        }
#pragma warning restore CS1998
    }
}
