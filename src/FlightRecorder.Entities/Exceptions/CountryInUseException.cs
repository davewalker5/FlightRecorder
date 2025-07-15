using System;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class CountryInUseException : Exception
    {
        public CountryInUseException()
        {
        }

        public CountryInUseException(string message) : base(message)
        {
        }

        public CountryInUseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
