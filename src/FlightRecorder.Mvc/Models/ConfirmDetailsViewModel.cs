using System;
using System.ComponentModel;

namespace FlightRecorder.Mvc.Models
{
    public class ConfirmDetailsViewModel
    {
        [DisplayName("Date")]
        public DateTime Date { get; set; }

        [DisplayName("Altitude")]
        public int Altitude { get; set; }

        [DisplayName("Location")]
        public string Location { get; set; }

        [DisplayName("My Flight")]
        public bool IsMyFlight { get; set; }

        [DisplayName("My Flight")]
        public string MyFlightText { get { return IsMyFlight ? "Yes" : "No"; }}

        [DisplayName("Flight Number")]
        public string FlightNumber { get; set; }

        [DisplayName("Embarkation")]
        public string Embarkation { get; set; }

        [DisplayName("Destination")]
        public string Destination { get; set; }

        [DisplayName("Airline")]
        public string Airline { get; set; }

        [DisplayName("Aircraft Registration")]
        public string Registration { get; set; }

        [DisplayName("Serial Number")]
        public string SerialNumber { get; set; }

        [DisplayName("Age")]
        public int? Age { get; set; }

        [DisplayName("Manufacturer")]
        public string Manufacturer { get; set; }

        [DisplayName("Model")]
        public string Model { get; set; }

        public string Action { get; set; }
    }
}
