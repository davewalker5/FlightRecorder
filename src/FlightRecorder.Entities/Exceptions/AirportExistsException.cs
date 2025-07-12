using System;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class AirportExistsException : Exception
    {
        public AirportExistsException()
        {
        }

        public AirportExistsException(string message) : base(message)
        {
        }

        public AirportExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
