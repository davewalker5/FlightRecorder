using System;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class AirlineExistsException : Exception
    {
        public AirlineExistsException()
        {
        }

        public AirlineExistsException(string message) : base(message)
        {
        }

        public AirlineExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
