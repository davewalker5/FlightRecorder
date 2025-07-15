using System;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class SightingNotFoundException : Exception
    {
        public SightingNotFoundException()
        {
        }

        public SightingNotFoundException(string message) : base(message)
        {
        }

        public SightingNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
