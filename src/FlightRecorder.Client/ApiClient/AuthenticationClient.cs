using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FlightRecorder.Client.Interfaces;
using FlightRecorder.Entities.Config;
using Microsoft.Extensions.Logging;

namespace FlightRecorder.Client.ApiClient
{
    public class AuthenticationClient : FlightRecorderClientBase, IAuthenticationClient
    {
        public AuthenticationClient(
            IFlightRecorderHttpClient client,
            FlightRecorderApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ICacheWrapper cache,
            ILogger<AuthenticationClient> logger)
            : base(client, settings, tokenProvider, cache, logger)
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
            var jsonCredentials = JsonSerializer.Serialize(credentials);

            // Send the request. The route is configured in appsettings.json
            var route = Settings.ApiRoutes.First(r => r.Name == "Authenticate").Route;
            var content = new StringContent(jsonCredentials, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync($"{Settings.ApiUrl}{route}", content, null, Logger);

            string token = null;
            if (response.IsSuccessStatusCode)
            {
                Logger.LogDebug($"Successfully authenticated as user {username}");

                // Read the token from the response body and instruct the provider to store it
                token = await response.Content.ReadAsStringAsync();
                TokenProvider.SetToken(token);
            }
            else
            {
                var message = $"{(int)response.StatusCode} : {response.ReasonPhrase}";
                Logger.LogDebug($"Authentication for user {username} failed : {message}");
                throw new AuthenticationException(message);
            }

            return token;
        }

        /// <summary>
        /// Log out by instructing the token provider to clear the token and clearing the authentication header
        /// </summary>
        public void ClearAuthentication()
        {
            TokenProvider.ClearToken();
        }

        /// <summary>
        /// Clear cached user attributes
        /// </summary>
        public void ClearCachedUserAttributes()
            => Cache.Remove(UserAttributesClient.CacheKey);
    }
}
