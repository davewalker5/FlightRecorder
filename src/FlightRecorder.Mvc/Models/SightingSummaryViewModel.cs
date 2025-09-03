using FlightRecorder.Entities.Db;

namespace FlightRecorder.Mvc.Models
{
    public class SightingSummaryViewModel
    {
        public string Message { get; set; }
        public Sighting Sighting { get; set; }
    }
}
