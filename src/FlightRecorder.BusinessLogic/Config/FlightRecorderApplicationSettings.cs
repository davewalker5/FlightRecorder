using FlightRecorder.Entities.Config;
using FlightRecorder.Entities.Logging;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.BusinessLogic.Config
{
    [ExcludeFromCodeCoverage]
    public class FlightRecorderApplicationSettings
    {
        public string Secret { get; set; }
        public int TokenLifespanMinutes { get; set; }
        public string LogFile { get; set; }
        public Severity MinimumLogLevel { get; set; }
        public string SightingsExportPath { get; set; }
        public string AirportsExportPath { get; set; }
        public string ReportsExportPath { get; set; }
        public List<ApiEndpoint> ApiEndpoints { get; set; }
        public List<ApiServiceKey> ApiServiceKeys { get; set; }
    }
}