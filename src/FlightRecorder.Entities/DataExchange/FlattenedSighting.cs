using FlightRecorder.Entities.Attributes;
using FlightRecorder.Entities.Db;
using System;
using System.Globalization;

namespace FlightRecorder.Entities.DataExchange
{
    public class FlattenedSighting : FlightRecorderEntityBase
    {
        public const string CsvRecordPattern = @"^""[a-zA-Z0-9-() \/']*"",(""[a-zA-Z0-9-() \/']+"",){6}""[0-9]+"",(""[a-zA-Z0-9-() \/']+"",){3}""[0-9]+\/[0-9]+\/[0-9]+"",""[a-zA-Z0-9-() \/']+"",""True|False""$";
        private const string DateTimeFormat = "dd/MM/yyyy";

        [Export("Callsign", 1)]
        public string Callsign { get; set; }

        [Export("Flight", 2)]
        public string FlightNumber { get; set; }

        [Export("Airline", 3)]
        public string Airline { get; set; }

        [Export("Registration", 4)]
        public string Registration { get; set; }

        [Export("Serial Number", 5)]
        public string SerialNumber { get; set; }

        [Export("Manufacturer", 6)]
        public string Manufacturer { get; set; }

        [Export("Type", 7)]
        public string Model { get; set; }

        [Export("Age", 8)]
        public string Age { get; set; }

        [Export("From", 9)]
        public string Embarkation { get; set; }

        [Export("To", 10)]
        public string Destination { get; set; }

        [Export("Height", 11)]
        public long Altitude { get; set; }

        [Export("Date", 12)]
        public DateTime Date { get; set; }

        [Export("Location", 13)]
        public string Location  { get; set; }

        [Export("My Flight", 14)]
        public bool IsMyFlight  { get; set; }

        public static FlattenedSighting FromCsv(string record)
        {
            string[] words = record.Split(["\",\""], StringSplitOptions.None);
            return new FlattenedSighting
            {
                Callsign = words[0].Substring(1),
                FlightNumber = words[1],
                Airline = words[2],
                Registration = words[3],
                SerialNumber = words[4],
                Manufacturer = words[5],
                Model = words[6],
                Age = words[7],
                Embarkation = words[8],
                Destination = words[9],
                Altitude = long.Parse(words[10]),
                Date = DateTime.ParseExact(words[11], DateTimeFormat, CultureInfo.CurrentCulture),
                Location = words[12][..^1],
                IsMyFlight = words[13].Equals("True", StringComparison.OrdinalIgnoreCase)
            };
        }
    }
}
