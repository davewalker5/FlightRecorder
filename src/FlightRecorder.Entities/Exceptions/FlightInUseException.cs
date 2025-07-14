using System;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class FlightInUseException : Exception
    {
        public FlightInUseException()
        {
        }

        public FlightInUseException(string message) : base(message)
        {
        }

        public FlightInUseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
