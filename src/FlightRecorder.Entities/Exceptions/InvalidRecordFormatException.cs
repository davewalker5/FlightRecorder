using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

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

        protected InvalidRecordFormatException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}

