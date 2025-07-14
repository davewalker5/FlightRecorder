using System;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class FlightNotFoundException : Exception
    {
        public FlightNotFoundException()
        {
        }

        public FlightNotFoundException(string message) : base(message)
        {
        }

        public FlightNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
