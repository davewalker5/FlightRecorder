namespace FlightRecorder.Mvc.Entities
{
    public class ModelStatistics
    {
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public int Sightings { get; set; }
        public int Flights { get; set; }
        public int Locations { get; set; }
        public int Aircraft { get; set; }
    }
}
