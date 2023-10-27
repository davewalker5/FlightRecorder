using FlightRecorder.Entities.Attributes;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Reporting
{
    [Keyless]
    [ExcludeFromCodeCoverage]
    public class ModelStatistics
    {
        [Export("Manufacturer", 2)]
        public string Manufacturer { get; set; }

        [Export("Model", 1)]
        public string Model { get; set; }

        [Export("Sightings", 3)]
        public int? Sightings { get; set; }

        [Export("Flights", 4)]
        public int? Flights { get; set; }

        [Export("Locations", 5)]
        public int? Locations { get; set; }

        [Export("Aircraft", 6)]
        public int? Aircraft { get; set; }
    }
}
