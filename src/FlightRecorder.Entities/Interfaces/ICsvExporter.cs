using FlightRecorder.Entities.DataExchange;
using System;
using System.Collections.Generic;

namespace FlightRecorder.Entities.Interfaces
{
    public interface ICsvExporter<T> where T : class
    {
        event EventHandler<ExportEventArgs<T>> RecordExport;
        void Export(IEnumerable<T> entities, string fileName, char separator);
    }
}
