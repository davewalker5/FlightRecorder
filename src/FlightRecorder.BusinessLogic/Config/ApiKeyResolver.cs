using System.IO;

namespace FlightRecorder.BusinessLogic.Config
{
    public static class ApiKeyResolver
    {
        /// <summary>
        /// Resolve an API key given the value from the configuration file
        /// </summary>
        /// <param name="configValue"></param>
        /// <returns></returns>
        public static string ResolveApiKey(string configValue)
        {
            string apiKey;

            // If the value from the configuration file is a valid file path, the keys are
            // stored separately. This separation allows the API keys not to be published
            // as part of the API Docker container image but read from a volume mount
            if (File.Exists(configValue))
            {
                apiKey = File.ReadAllText(configValue);
            }
            else
            {
                // Not a path to a file, so just return the configuration value as the key
                apiKey = configValue;
            }

            return apiKey;
        }

        /// <summary>
        /// Resolve all the API key definitions in the supplied application settings
        /// </summary>
        /// <param name="settings"></param>
        public static void ResolveAllApiKeys(FlightRecorderApplicationSettings settings)
        {

            // Iterate over the service API key definitions
            foreach (var service in settings.ApiServiceKeys)
            {
                // Resolve the key for the current service
                service.Key = ResolveApiKey(service.Key);
            }
        }
    }
}
