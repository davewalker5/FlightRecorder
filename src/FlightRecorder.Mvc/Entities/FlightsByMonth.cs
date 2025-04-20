namespace FlightRecorder.Mvc.Entities
{
    public class FlightsByMonth
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Sightings { get; set; }
        public int Flights { get; set; }
    }
}
