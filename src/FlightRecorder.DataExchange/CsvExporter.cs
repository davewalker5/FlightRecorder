using FlightRecorder.Entities.DataExchange;
using FlightRecorder.Entities.Db;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FlightRecorder.DataExchange
{
    public class CsvExporter
    {
        private readonly string[] ColumnHeaders =
        {
            "Flight",
            "Airline",
            "Registration",
            "Serial Number",
            "Manufacturer",
            "Type",
            "Age",
            "From",
            "To",
            "Height",
            "Date",
            "Location"
        };

        public EventHandler<SightingDataExchangeEventArgs> RecordExport;

        /// <summary>
        /// Export the specified collection of sightings to a CSV file
        /// </summary>
        /// <param name="sightings"></param>
        /// <param name="file"></param>
        public void Export(IEnumerable<Sighting> sightings, string file)
        {
            IEnumerable<FlattenedSighting> flattened = sightings.Flatten();

            using (StreamWriter writer = new StreamWriter(file, false, Encoding.UTF8))
            {
                string headers = string.Join(",", ColumnHeaders);
                writer.WriteLine(headers);

                int count = 0;
                foreach (FlattenedSighting sighting in flattened)
                {
                    writer.WriteLine(sighting.ToCsv());
                    count++;
                    RecordExport?.Invoke(this, new SightingDataExchangeEventArgs { RecordCount = count, Sighting = sighting });
                }
            }
        }
    }
}
