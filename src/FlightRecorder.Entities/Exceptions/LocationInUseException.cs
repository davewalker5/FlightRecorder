using System;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class LocationInUseException : Exception
    {
        public LocationInUseException()
        {
        }

        public LocationInUseException(string message) : base(message)
        {
        }

        public LocationInUseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
