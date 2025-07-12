using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using FlightRecorder.Entities.Attributes;

namespace FlightRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public partial class Flight
    {
        [Key]
        public long Id { get; set; }

        [DisplayName("Airline")]
        [Required(ErrorMessage = "You must specify an airline")]
        public long AirlineId { get; set; }

        [DisplayName("Registration")]
        [Required(ErrorMessage = "You must provide a flight number")]
        public string Number { get; set; }

        [IATACode]
        [DisplayName("Embarkation")]
        [Required(ErrorMessage = "You must provide a point of embarkation")]
        public string Embarkation { get; set; }

        [IATACode]
        [DisplayName("Destination")]
        [Required(ErrorMessage = "You must provide a destination")]
        public string Destination { get; set; }

        public virtual Airline Airline { get; set; }

        public string Route { get { return $"{Embarkation} - {Destination}"; } }
    }
}
