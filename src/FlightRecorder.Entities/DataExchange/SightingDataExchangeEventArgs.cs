using System;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.DataExchange
{
    [ExcludeFromCodeCoverage]
    public class SightingDataExchangeEventArgs : EventArgs
    {
        public long RecordCount { get; set; }
        public FlattenedSighting Sighting { get; set; }
    }
}
