using System;
using System.Globalization;

namespace FlightRecorder.Entities.DataExchange
{
    public class FlattenedSighting
    {
        public const string CsvRecordPattern = @"^(""[a-zA-Z0-9-() \/']+"",){6}""[0-9]+"",(""[a-zA-Z0-9-() \/']+"",){3}""[0-9]+\/[0-9]+\/[0-9]+"",""[a-zA-Z0-9-() \/']+""$";
        private const string DateTimeFormat = "dd/MM/yyyy";

        public string FlightNumber { get; set; }
        public string Airline { get; set; }
        public string Registration { get; set; }
        public string SerialNumber { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public long Age { get; set; }
        public string Embarkation { get; set; }
        public string Destination { get; set; }
        public long Altitude { get; set; }
        public DateTime Date { get; set; }
        public string Location  { get; set; }

        public string ToCsv()
        {
            string date = Date.ToString(DateTimeFormat);
            string representation = $"\"{FlightNumber}\",\"{Airline}\",\"{Registration}\",\"{SerialNumber}\",\"{Manufacturer}\",\"{Model}\",\"{Age}\",\"{Embarkation}\",\"{Destination}\",\"{Altitude}\",\"{date}\",\"{Location}\"";
            return representation;
        }

        public static FlattenedSighting FromCsv(string record)
        {
            string[] words = record.Split(new string[] { "\",\"" }, StringSplitOptions.None);
            return new FlattenedSighting
            {
                FlightNumber = words[0].Substring(1),
                Airline = words[1],
                Registration = words[2],
                SerialNumber = words[3],
                Manufacturer = words[4],
                Model = words[5],
                Age = long.Parse(words[6]),
                Embarkation = words[7],
                Destination = words[8],
                Altitude = long.Parse(words[9]),
                Date = DateTime.ParseExact(words[10], DateTimeFormat, CultureInfo.CurrentCulture),
                Location = words[11].Substring(0, words[11].Length - 1)
            };
        }
    }
}
