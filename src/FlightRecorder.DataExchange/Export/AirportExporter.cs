using FlightRecorder.Entities.DataExchange;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.DataExchange.Export
{
    [ExcludeFromCodeCoverage]
    public class AirportExporter : IAirportExporter
    {
        public event EventHandler<ExportEventArgs<FlattenedAirport>> RecordExport;

        /// <summary>
        /// Export the specified collection of airports to a CSV file
        /// </summary>
        /// <param name="airports"></param>
        /// <param name="file"></param>
        public void Export(IEnumerable<Airport> airports, string file)
        {
            // Flatten the collection of airports
            IEnumerable<FlattenedAirport> flattened = airports.Flatten();

            // Configure an exporter to export them
            var exporter = new CsvExporter<FlattenedAirport>();
            exporter.RecordExport += OnRecordExported;

            // Export the records
            exporter.Export(flattened, file, ',');
        }

        /// <summary>
        /// Handler for airport export notifications
        /// </summary>
        /// <param name="_"></param>
        /// <param name="e"></param>
        private void OnRecordExported(object _, ExportEventArgs<FlattenedAirport> e)
        {
            RecordExport?.Invoke(this, e);
        }
    }
}
