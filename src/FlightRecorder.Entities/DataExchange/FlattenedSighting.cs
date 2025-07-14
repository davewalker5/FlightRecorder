using FlightRecorder.Entities.Attributes;
using FlightRecorder.Entities.Db;
using System;
using System.Globalization;

namespace FlightRecorder.Entities.DataExchange
{
    public class FlattenedSighting : FlightRecorderEntityBase
    {
        // TODO : Add IsMyFlight to the Regex pattern
        public const string CsvRecordPattern = @"^(""[a-zA-Z0-9-() \/']+"",){6}""[0-9]+"",(""[a-zA-Z0-9-() \/']+"",){3}""[0-9]+\/[0-9]+\/[0-9]+"",""[a-zA-Z0-9-() \/']+"",""True|False""$";
        private const string DateTimeFormat = "dd/MM/yyyy";

        [Export("Flight", 1)]
        public string FlightNumber { get; set; }

        [Export("Airline", 2)]
        public string Airline { get; set; }

        [Export("Registration", 3)]
        public string Registration { get; set; }

        [Export("Serial Number", 4)]
        public string SerialNumber { get; set; }

        [Export("Manufacturer", 5)]
        public string Manufacturer { get; set; }

        [Export("Type", 6)]
        public string Model { get; set; }

        [Export("Age", 7)]
        public string Age { get; set; }

        [Export("From", 8)]
        public string Embarkation { get; set; }

        [Export("To", 9)]
        public string Destination { get; set; }

        [Export("Height", 10)]
        public long Altitude { get; set; }

        [Export("Date", 11)]
        public DateTime Date { get; set; }

        [Export("Location", 12)]
        public string Location  { get; set; }

        [Export("My Flight", 13)]
        public bool IsMyFlight  { get; set; }

        public static FlattenedSighting FromCsv(string record)
        {
            string[] words = record.Split(["\",\""], StringSplitOptions.None);
            return new FlattenedSighting
            {
                FlightNumber = words[0].Substring(1),
                Airline = words[1],
                Registration = words[2],
                SerialNumber = words[3],
                Manufacturer = words[4],
                Model = words[5],
                Age = words[6],
                Embarkation = words[7],
                Destination = words[8],
                Altitude = long.Parse(words[9]),
                Date = DateTime.ParseExact(words[10], DateTimeFormat, CultureInfo.CurrentCulture),
                Location = words[11][..^1],
                IsMyFlight = words[12].Equals("True", StringComparison.OrdinalIgnoreCase)
            };
        }
    }
}
