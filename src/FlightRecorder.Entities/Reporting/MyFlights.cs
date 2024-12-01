using FlightRecorder.Entities.Attributes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Reporting
{
    [Keyless]
    [ExcludeFromCodeCoverage]
    public class MyFlights
    {
        [Export("Date", 1)]
        public DateTime Date { get; set; }

        [Export("Airline", 2)]
        public string Airline { get; set; }

        [Export("Number", 3)]
        public string Number { get; set; }

        [Export("Embarkation", 4)]
        public string Embarkation { get; set; }

        [Export("Destination", 5)]
        public string Destination { get; set; }

        [Export("Registration", 6)]
        public string Registration { get; set; }

        [Export("Model", 7)]
        public string Model { get; set; }

        [Export("Manufacturer", 7)]
        public string Manufacturer { get; set; }
    }
}
