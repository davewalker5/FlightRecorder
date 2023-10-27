using FlightRecorder.Entities.Attributes;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Reporting
{
    [Keyless]
    [ExcludeFromCodeCoverage]
    public class ManufacturerStatistics
    {
        [Export("Manufacturer", 1)]
        public string Name { get; set; }

        [Export("Sightings", 2)]
        public int? Sightings { get; set; }

        [Export("Flights", 3)]
        public int? Flights { get; set; }

        [Export("Locations", 4)]
        public int? Locations { get; set; }

        [Export("Aircraft", 5)]
        public int? Aircraft { get; set; }

        [Export("Models", 6)]
        public int? Models { get; set; }
    }
}
