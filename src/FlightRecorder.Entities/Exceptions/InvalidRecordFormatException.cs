using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace FlightRecorder.Entities.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class InvalidRecordFormatException : Exception
    {
        public InvalidRecordFormatException()
        {
        }

        public InvalidRecordFormatException(string message) : base(message)
        {
        }

        public InvalidRecordFormatException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

