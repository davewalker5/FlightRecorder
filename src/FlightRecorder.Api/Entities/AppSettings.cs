namespace FlightRecorder.Api.Entities
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public int TokenLifespanMinutes { get; set; }
        public string SightingsExportPath { get; set; }
        public string AirportsExportPath { get; set; }
        public string ReportsExportPath { get; set; }
    }
}
