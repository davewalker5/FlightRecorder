using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace FlightRecorder.Entities.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class UserExistsException : Exception
    {
        public UserExistsException()
        {
        }

        public UserExistsException(string message) : base(message)
        {
        }

        public UserExistsException(string message, Exception inner) : base(message, inner)
        {
        }

        protected UserExistsException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }
    }
}
