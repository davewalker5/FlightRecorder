using System;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class AircraftNotFoundException : Exception
    {
        public AircraftNotFoundException()
        {
        }

        public AircraftNotFoundException(string message) : base(message)
        {
        }

        public AircraftNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
