using System;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class CountryExistsException : Exception
    {
        public CountryExistsException()
        {
        }

        public CountryExistsException(string message) : base(message)
        {
        }

        public CountryExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
