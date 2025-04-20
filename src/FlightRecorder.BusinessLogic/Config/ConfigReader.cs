using FlightRecorder.Entities.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FlightRecorder.BusinessLogic.Config
{
    public abstract class ConfigReader<T> : IConfigReader<T> where T : class
    {
        /// <summary>
        /// Load and return the application settings from the named JSON-format application settings file
        /// </summary>
        /// <param name="jsonFileName"></param>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public virtual T Read(string jsonFileName, string sectionName)
        {
            // Set up the configuration reader
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile(jsonFileName)
                .Build();

            // Read the application settings section
            IConfigurationSection section = configuration.GetSection(sectionName);
            var settings = section.Get<T>();

            return settings;
        }
    }
}
