using FlightRecorder.Entities.DataExchange;
using FlightRecorder.Entities.Db;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace FlightRecorder.DataExchange
{
    [ExcludeFromCodeCoverage]
    public class AirportExporter
    {
        private readonly string[] ColumnHeaders =
        {
            "Code",
            "Name",
            "Country"
        };

        public EventHandler<AirportDataExchangeEventArgs> RecordExport;

        /// <summary>
        /// Export the specified collection of airports to a CSV file
        /// </summary>
        /// <param name="airports"></param>
        /// <param name="file"></param>
        public void Export(IEnumerable<Airport> airports, string file)
        {
            IEnumerable<FlattenedAirport> flattened = airports.Flatten();

            using (StreamWriter writer = new StreamWriter(file, false, Encoding.UTF8))
            {
                string headers = string.Join(",", ColumnHeaders);
                writer.WriteLine(headers);

                int count = 0;
                foreach (FlattenedAirport airport in flattened)
                {
                    writer.WriteLine(airport.ToCsv());
                    count++;
                    RecordExport?.Invoke(this, new AirportDataExchangeEventArgs { RecordCount = count, Airport = airport });
                }
            }
        }
    }
}
