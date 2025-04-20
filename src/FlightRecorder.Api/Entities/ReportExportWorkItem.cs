using System;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Api.Entities
{
    [ExcludeFromCodeCoverage]
    public class ReportExportWorkItem : BackgroundWorkItem
    {
        public ReportType Type { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string FileName { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()}, FileName = {FileName}";
        }
    }
}
