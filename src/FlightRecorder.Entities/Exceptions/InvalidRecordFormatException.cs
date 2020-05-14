using System;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Exceptions
{
    [Serializable]
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

