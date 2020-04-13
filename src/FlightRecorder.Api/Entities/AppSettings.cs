namespace FlightRecorder.Api.Entities
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public int TokenLifespanMinutes { get; set; }
    }
}
