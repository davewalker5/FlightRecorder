using System.Collections.Generic;
using FlightRecorder.Entities.Logging;

namespace FlightRecorder.Entities.Config
{
    public class FlightRecorderApplicationSettings
    {
        public string Secret { get; set; }
        public int TokenLifespanMinutes { get; set; }
        public string SightingsExportPath { get; set; }
        public string AirportsExportPath { get; set; }
        public string ReportsExportPath { get; set; }
        public string LogFile { get; set; }
        public Severity MinimumLogLevel { get; set; }
        public List<ApiEndpoint> ApiEndpoints { get; set; }
        public List<ApiServiceKey> ApiServiceKeys { get; set; }
        public string ApiUrl { get; set; }
        public string ApiDateFormat { get; set; }
        public List<ApiRoute> ApiRoutes { get; set; }
        public int SearchPageSize { get; set; }
        public int CacheLifetimeSeconds { get; set; }
        public string DateTimeFormat { get; set; }
        public string DefaultLocationAttribute { get; set; }
        public bool UseCustomErrorPageInDevelopment { get; set; }
    }
}
