using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using FlightRecorder.Entities.DataExchange;

namespace FlightRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public partial class Sighting : FlightRecorderEntityBase
    {
        [Key]
        public long Id { get; set; }
        public long LocationId { get; set; }
        public long FlightId { get; set; }
        public long AircraftId { get; set; }
        public long Altitude { get; set; }
        public DateTime Date { get; set; }

        public bool IsMyFlight { get; set; }

        public virtual Aircraft Aircraft { get; set; }
        public virtual Flight Flight { get; set; }
        public virtual Location Location { get; set; }

        public string FormattedDate { get { return Date.ToString("dd-MMM-yyyy"); } }

        public FlattenedSighting Flatten()
        {
            return new FlattenedSighting
            {
                FlightNumber = Flight.Number,
                Airline = Flight.Airline.Name,
                Registration = Aircraft.Registration,
                SerialNumber = Aircraft.SerialNumber ?? "",
                Manufacturer = Aircraft.Model?.Manufacturer?.Name ?? "",
                Model = Aircraft.Model?.Name ?? "",
                Age = (Aircraft.Manufactured > 0) ? (DateTime.Now.Year - Aircraft.Manufactured).ToString() : "",
                Embarkation = Flight.Embarkation,
                Destination = Flight.Destination,
                Altitude = Altitude,
                Date = Date,
                Location = Location.Name,
                IsMyFlight = IsMyFlight
            };
        }
    }
}
