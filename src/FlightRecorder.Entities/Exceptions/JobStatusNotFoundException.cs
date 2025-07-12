using System;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class JobStatusNotFoundException : Exception
    {
        public JobStatusNotFoundException()
        {
        }

        public JobStatusNotFoundException(string message) : base(message)
        {
        }

        public JobStatusNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
