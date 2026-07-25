using FlightRecorder.Entities.Attributes;
using FlightRecorder.Entities.Db;
using System;
using System.Globalization;

namespace FlightRecorder.Entities.DataExchange
{
    public class FlattenedSighting : FlightRecorderEntityBase
    {
        public const string CsvRecordPattern = @"^""[a-zA-Z0-9-() \/']*"",(""[a-zA-Z0-9-() \/']+"",){2}""[a-fA-F0-9]*"",(""[a-zA-Z0-9-() \/']+"",){4}""[0-9]+"",(""[a-zA-Z0-9-() \/']+"",){3}""[0-9]+\/[0-9]+\/[0-9]+"",""[a-zA-Z0-9-() \/']+"",""True|False""$";
        private const string DateTimeFormat = "dd/MM/yyyy";

        [Export("Callsign", 1)]
        public string Callsign { get; set; }

        [Export("Flight", 2)]
        public string FlightNumber { get; set; }

        [Export("Airline", 3)]
        public string Airline { get; set; }

        [Export("Aircraft Address", 4)]
        public string AircraftAddress { get; set; }

        [Export("Registration", 5)]
        public string Registration { get; set; }

        [Export("Serial Number", 6)]
        public string SerialNumber { get; set; }

        [Export("Manufacturer", 7)]
        public string Manufacturer { get; set; }

        [Export("Type", 8)]
        public string Model { get; set; }

        [Export("Age", 9)]
        public string Age { get; set; }

        [Export("From", 10)]
        public string Embarkation { get; set; }

        [Export("To", 11)]
        public string Destination { get; set; }

        [Export("Height", 12)]
        public long Altitude { get; set; }

        [Export("Date", 13)]
        public DateTime Date { get; set; }

        [Export("Location", 14)]
        public string Location  { get; set; }

        [Export("My Flight", 15)]
        public bool IsMyFlight  { get; set; }

        public static FlattenedSighting FromCsv(string record)
        {
            string[] words = record.Split(["\",\""], StringSplitOptions.None);
            return new FlattenedSighting
            {
                Callsign = words[0].Substring(1),
                FlightNumber = words[1],
                Airline = words[2],
                AircraftAddress = words[3],
                Registration = words[4],
                SerialNumber = words[5],
                Manufacturer = words[6],
                Model = words[7],
                Age = words[8],
                Embarkation = words[9],
                Destination = words[10],
                Altitude = long.Parse(words[11]),
                Date = DateTime.ParseExact(words[12], DateTimeFormat, CultureInfo.CurrentCulture),
                Location = words[13][..^1],
                IsMyFlight = words[14].Equals("True", StringComparison.OrdinalIgnoreCase)
            };
        }
    }
}
