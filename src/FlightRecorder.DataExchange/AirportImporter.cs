using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Entities.DataExchange;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Exceptions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FlightRecorder.DataExchange
{
    [ExcludeFromCodeCoverage]
    public class AirportImporter: IDataImporter
    {
        public EventHandler<AirportDataExchangeEventArgs> RecordImport;

        /// <summary>
        /// Import the contents of the CSV file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="context"></param>
        public void Import(string file, FlightRecorderFactory factory)
        {
            Regex regex = new Regex(FlattenedAirport.CsvRecordPattern, RegexOptions.Compiled);

            using (StreamReader reader = new StreamReader(file, Encoding.UTF8))
            {
                int count = 0;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    count++;

                    if (count > 1)
                    {
                        // Check the line matches the pattern required for successful import
                        bool matches = regex.Matches(line).Any();
                        if (!matches)
                        {
                            Console.WriteLine(line);
                            string message = $"Invalid record format at line {count} of {file}";
                            throw new InvalidRecordFormatException(message);
                        }

                        // Inflate the CSV record to an airport and store it in the database
                        FlattenedAirport airport = FlattenedAirport.FromCsv(line);
                        Country country = factory.Countries.Add(airport.Country);
                        factory.Airports.Add(airport.Code, airport.Name, airport.Country);

                        RecordImport?.Invoke(this, new AirportDataExchangeEventArgs { RecordCount = count - 1, Airport = airport });
                    }
                }
            }
        }
    }
}
