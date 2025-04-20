using System;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.DataExchange
{
    [ExcludeFromCodeCoverage]
    public class AirportDataExchangeEventArgs : EventArgs
    {
        public long RecordCount { get; set; }
        public FlattenedAirport Airport { get; set; }
    }
}
