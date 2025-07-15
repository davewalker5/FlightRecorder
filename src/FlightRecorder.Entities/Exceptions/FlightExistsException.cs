using System;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class FlightExistsException : Exception
    {
        public FlightExistsException()
        {
        }

        public FlightExistsException(string message) : base(message)
        {
        }

        public FlightExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
