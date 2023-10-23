using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Reporting
{
    [Keyless]
    [ExcludeFromCodeCoverage]
    public class FlightsByMonth
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int? Sightings { get; set; }
        public int? Flights { get; set; }
    }
}
