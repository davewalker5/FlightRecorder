using FlightRecorder.Entities.Attributes;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Reporting
{
    [Keyless]
    [ExcludeFromCodeCoverage]
    public class LocationStatistics
    {
        [Export("Location", 1)]
        public string Name { get; set; }

        [Export("Sightings", 2)]
        public int? Sightings { get; set; }

        [Export("Flights", 3)]
        public int? Flights { get; set; }

        [Export("Aircraft", 4)]
        public int? Aircraft { get; set; }

        [Export("Models", 5)]
        public int? Models { get; set; }

        [Export("Manufacturers", 6)]
        public int? Manufacturers { get; set; }
    }
}
