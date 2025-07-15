using System;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class FlightRecorderApiRequestException : Exception
    {
        public FlightRecorderApiRequestException()
        {
        }

        public FlightRecorderApiRequestException(string message) : base(message)
        {
        }

        public FlightRecorderApiRequestException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
