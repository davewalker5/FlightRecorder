using System;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class AircraftInUseException : Exception
    {
        public AircraftInUseException()
        {
        }

        public AircraftInUseException(string message) : base(message)
        {
        }

        public AircraftInUseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

