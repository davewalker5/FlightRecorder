﻿using FlightRecorder.Mvc.Configuration;
using FlightRecorder.Mvc.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace FlightRecorder.Mvc.Api
{
    public class AuthenticationClient : FlightRecorderClientBase
    {
        public AuthenticationClient(HttpClient client, IOptions<AppSettings> settings, IHttpContextAccessor accessor, ICacheWrapper cache)
            : base(client, settings, accessor, cache)
        {
        }

        /// <summary>
        /// Authenticate with the service and, if successful, return the JWT token
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<string> AuthenticateAsync(string username, string password)
        {
            // Construct the JSON containing the user credentials
            dynamic credentials = new { UserName = username, Password = password };
            string jsonCredentials = JsonConvert.SerializeObject(credentials);

            // Send the request. The route is configured in appsettings.json
            string route = Settings.Value.ApiRoutes.First(r => r.Name == "Authenticate").Route;
            StringContent content = new StringContent(jsonCredentials, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await Client.PostAsync(route, content);

            string token = null;
            if (response.IsSuccessStatusCode)
            {
                // Read the token from the response body and set up the default request
                // authentication header
                token = await response.Content.ReadAsStringAsync();
                SetAuthenticationHeader(token);
            }

            return token;
        }

        /// <summary>
        /// Clear cached user attributes
        /// </summary>
        public void ClearCachedUserAttributes()
            => Cache.Remove(UserAttributesClient.CacheKey);
    }
}
