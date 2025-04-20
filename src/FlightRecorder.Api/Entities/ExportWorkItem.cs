using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Api.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportWorkItem : BackgroundWorkItem
    {
        public string FileName { get; set; }
        public override string ToString()
        {
            return $"{base.ToString()}, FileName = {FileName}";
        }
    }
}
