using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Entities.DataExchange;
using FlightRecorder.Entities.Exceptions;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlightRecorder.DataExchange
{
    public class CsvImporter : IDataImporter
    {
        public EventHandler<SightingDataExchangeEventArgs> RecordImport;

        /// <summary>
        /// Import the contents of the CSV file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="context"></param>
        public async Task Import(string file, FlightRecorderFactory factory)
        {
            Regex regex = new Regex(FlattenedSighting.CsvRecordPattern, RegexOptions.Compiled);

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

                        // Inflate the CSV record to a sighting and store it in the database
                        FlattenedSighting sighting = FlattenedSighting.FromCsv(line);
                        await factory.Sightings.AddAsync(sighting);

                        RecordImport?.Invoke(this, new SightingDataExchangeEventArgs { RecordCount = count - 1, Sighting = sighting });
                    }
                }
            }
        }
    }
}
