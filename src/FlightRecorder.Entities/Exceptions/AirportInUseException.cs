using System;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class AirportInUseException : Exception
    {
        public AirportInUseException()
        {
        }

        public AirportInUseException(string message) : base(message)
        {
        }

        public AirportInUseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
