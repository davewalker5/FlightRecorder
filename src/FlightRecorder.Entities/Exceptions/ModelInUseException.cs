using System;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class ModelInUseException : Exception
    {
        public ModelInUseException()
        {
        }

        public ModelInUseException(string message) : base(message)
        {
        }

        public ModelInUseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
