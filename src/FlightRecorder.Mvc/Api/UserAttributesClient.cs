using FlightRecorder.Mvc.Configuration;
using FlightRecorder.Mvc.Entities;
using FlightRecorder.Mvc.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FlightRecorder.Mvc.Api
{
    public class UserAttributesClient : FlightRecorderClientBase
    {
        private const string RouteKey = "UserAttributes";
        public const string CacheKey = "UserAttributes";

        public UserAttributesClient(HttpClient client, IOptions<AppSettings> settings, IHttpContextAccessor accessor, ICacheWrapper cache)
            : base(client, settings, accessor, cache)
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
            var attributes = Cache.Get<List<UserAttributeValue>>(CacheKey);
            if ((attributes == null) || !useCachedValue)
            {
                var route = @$"{Settings.Value.ApiRoutes.First(r => r.Name == RouteKey).Route}/{userName}";
                var json = await SendDirectAsync(route, null, HttpMethod.Get);
                attributes = [.. JsonConvert.DeserializeObject<List<UserAttributeValue>>(json, JsonSettings)];
                Cache.Set(CacheKey, attributes, Settings.Value.CacheLifetimeSeconds);
            }
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
