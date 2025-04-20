using FlightRecorder.Entities.Attributes;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Reporting
{
    [Keyless]
    [ExcludeFromCodeCoverage]
    public class FlightsByMonth
    {
        [Export("Year", 1)]
        public int Year { get; set; }

        [Export("Month", 2)]
        public int Month { get; set; }

        [Export("Sightings", 3)]
        public int? Sightings { get; set; }

        [Export("Flights", 4)]
        public int? Flights { get; set; }
    }
}
