using FlightRecorder.Entities.Interfaces;

namespace FlightRecorder.BusinessLogic.Config
{
    public class FlightRecorderConfigReader : ConfigReader<FlightRecorderApplicationSettings>, IConfigReader<FlightRecorderApplicationSettings>
    {
        /// <summary>
        /// Load and return the application settings from the named JSON-format application settings file
        /// </summary>
        /// <param name="jsonFileName"></param>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public override FlightRecorderApplicationSettings Read(string jsonFileName, string sectionName)
        {
            // Read the settings
            var settings = base.Read(jsonFileName, sectionName);
            if (settings != null)
            {
                // Resolve all the API keys for services where the key is held in a separate file
                ApiKeyResolver.ResolveAllApiKeys(settings);
            }

            return settings;
        }
    }
}
