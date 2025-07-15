using System;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class AirlineInUseException : Exception
    {
        public AirlineInUseException()
        {
        }

        public AirlineInUseException(string message) : base(message)
        {
        }

        public AirlineInUseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
