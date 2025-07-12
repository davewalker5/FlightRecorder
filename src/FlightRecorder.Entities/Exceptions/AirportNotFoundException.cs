using System;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class AirportNotFoundException : Exception
    {
        public AirportNotFoundException()
        {
        }

        public AirportNotFoundException(string message) : base(message)
        {
        }

        public AirportNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
