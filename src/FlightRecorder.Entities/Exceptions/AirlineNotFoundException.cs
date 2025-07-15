using System;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class AirlineNotFoundException : Exception
    {
        public AirlineNotFoundException()
        {
        }

        public AirlineNotFoundException(string message) : base(message)
        {
        }

        public AirlineNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
