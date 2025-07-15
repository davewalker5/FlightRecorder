using System;
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
    public class UserAttributesClient : FlightRecorderClientBase, IUserAttributesClient
    {
        private const string RouteKey = "UserAttributes";
        public const string CacheKey = "UserAttributes";

        public UserAttributesClient(
            IFlightRecorderHttpClient client,
            FlightRecorderApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ICacheWrapper cache,
            ILogger<UserAttributesClient> logger)
            : base(client, settings, tokenProvider, cache, logger)
        {
        }

        /// <summary>
        /// Return the user attributes for the specified user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="useCachedValue"></param>
        /// <returns></returns>
        public async Task<List<UserAttributeValue>> GetUserAttributesAsync(string userName, bool useCachedValue)
        {
            Logger.LogDebug($"Retrieving attributes for user {userName}");

            var attributes = Cache.Get<List<UserAttributeValue>>(CacheKey);
            if ((attributes == null) || !useCachedValue)
            {
                var route = @$"{Settings.ApiRoutes.First(r => r.Name == RouteKey).Route}/{userName}";
                var json = await SendDirectAsync(route, null, HttpMethod.Get);
                attributes = Deserialize<List<UserAttributeValue>>(json);
                Cache.Set(CacheKey, attributes, Settings.CacheLifetimeSeconds);
                
            }

            Logger.LogDebug($"{attributes?.Count ?? 0} attribute(s) retrieved for user {userName}");
            return attributes;
        }

        /// <summary>
        /// Get a chached user attribute object
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public UserAttributeValue GetCachedUserAttribute(string attributeName)
        {
            UserAttributeValue value = null;
            var attributes = Cache.Get<List<UserAttributeValue>>(CacheKey);
            if (attributes != null)
            {
                value = attributes.FirstOrDefault(x => x.UserAttribute.Name.Equals(attributeName, StringComparison.OrdinalIgnoreCase));
            }
            return value;
        }
    }
}
