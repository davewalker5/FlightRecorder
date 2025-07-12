using System;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class AircraftExistsException : Exception
    {
        public AircraftExistsException()
        {
        }

        public AircraftExistsException(string message) : base(message)
        {
        }

        public AircraftExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

