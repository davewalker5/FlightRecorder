using FlightRecorder.Entities.DataExchange;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Interfaces;
using System;
using System.Collections.Generic;

namespace FlightRecorder.DataExchange.Export
{
    public class SightingsExporter : ISightingsExporter
    {
        public event EventHandler<ExportEventArgs<FlattenedSighting>> RecordExport;

        /// <summary>
        /// Export the specified collection of sightings to a CSV file
        /// </summary>
        /// <param name="sightings"></param>
        /// <param name="file"></param>
        public void Export(IEnumerable<Sighting> sightings, string file)
        {
            // Flatten the collection of sightings
            IEnumerable<FlattenedSighting> flattened = sightings.Flatten();

            // Configure an exporter to export them
            var exporter = new CsvExporter<FlattenedSighting>();
            exporter.RecordExport += OnRecordExported;

            // Export the records
            exporter.Export(flattened, file, ',');
        }

        /// <summary>
        /// Handler for sighting export notifications
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRecordExported(object _, ExportEventArgs<FlattenedSighting> e)
        {
            RecordExport?.Invoke(this, e);
        }
    }
}
