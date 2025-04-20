using System;

namespace FlightRecorder.Mvc.Entities
{
    public class Sighting
    {
        public int Id { get; set; }
        public int LocationId { get; set; }
        public int FlightId { get; set; }
        public int AircraftId { get; set; }
        public int Altitude { get; set; }
        public DateTime Date { get; set; }

        public Aircraft Aircraft { get; set; }
        public Flight Flight { get; set; }
        public Location Location { get; set; }

        public string FormattedDate { get { return Date.ToString("dd-MMM-yyyy"); } }
    }
}
