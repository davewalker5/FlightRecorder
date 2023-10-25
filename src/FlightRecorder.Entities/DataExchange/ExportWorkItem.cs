using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.DataExchange
{
    [ExcludeFromCodeCoverage]
    public class ExportWorkItem : BackgroundWorkItem
    {
        public string FileName { get; set; }
    }
}
