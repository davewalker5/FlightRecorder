﻿using Microsoft.EntityFrameworkCore;

namespace FlightRecorder.Entities.Reporting
{
    [Keyless]
    public class LocationStatistics
    {
        public string Name { get; set; }
        public int? Sightings { get; set; }
        public int? Flights { get; set; }
        public int? Aircraft { get; set; }
        public int? Models { get; set; }
        public int? Manufacturers { get; set; }
    }
}
