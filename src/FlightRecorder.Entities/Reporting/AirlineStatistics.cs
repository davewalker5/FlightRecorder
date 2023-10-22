using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Reporting
{
    [Keyless]
    [ExcludeFromCodeCoverage]
    public class AirlineStatistics
    {
        public string Name { get; set; }
        public int? Sightings { get; set; }
        public int? Flights { get; set; }
        public int? Locations { get; set; }
        public int? Aircraft { get; set; }
        public int? Models { get; set; }
        public int? Manufacturers { get; set; }
    }
}
