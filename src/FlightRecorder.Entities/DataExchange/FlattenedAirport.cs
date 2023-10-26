using FlightRecorder.Entities.Attributes;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.DataExchange
{
    [ExcludeFromCodeCoverage]
    public class FlattenedAirport
    {
        public const string CsvRecordPattern = @"^""[a-zA-Z]{3}"","".*"","".*""$";

        [Export("Code", 1)]
        public string Code { get; set; }

        [Export("Name", 2)]
        public string Name { get; set; }

        [Export("Country", 3)]
        public string Country { get; set; }

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
