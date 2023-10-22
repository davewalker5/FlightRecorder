using System;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.DataExchange
{
    [ExcludeFromCodeCoverage]
    public class FlattenedAirport
    {
        public const string CsvRecordPattern = @"^""[a-zA-Z]{3}"","".*"","".*""$";

        public string Code { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }

        public string ToCsv()
        {
            string representation = $"\"{Code}\",\"{Name}\",\"{Country}\"";
            return representation;
        }

        public static FlattenedAirport FromCsv(string record)
        {
            string[] words = record.Split(new string[] { "\",\"" }, StringSplitOptions.None);
            return new FlattenedAirport
            {
                Code = words[0].Replace("\"", "").Trim(),
                Name = words[1].Replace("\"", "").Trim(),
                Country = words[2].Replace("\"", "").Trim()
            };
        }
    }
}
