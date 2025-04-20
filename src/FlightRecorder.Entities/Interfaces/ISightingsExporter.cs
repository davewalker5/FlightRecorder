using FlightRecorder.Entities.DataExchange;
using FlightRecorder.Entities.Db;
using System;
using System.Collections.Generic;

namespace FlightRecorder.Entities.Interfaces
{
    public interface ISightingsExporter
    {
        event EventHandler<ExportEventArgs<FlattenedSighting>> RecordExport;

        void Export(IEnumerable<Sighting> sightings, string file);
    }
}