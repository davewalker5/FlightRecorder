using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FlightRecorder.Mvc.Entities
{
    public class Flight
    {
        public int Id { get; set; }

        [DisplayName("Model")]
        [Required(ErrorMessage = "You must specify an airline")]
        public int AirlineId { get; set; }

        [DisplayName("Registration")]
        [Required(ErrorMessage = "You must provide a flight number")]
        public string Number { get; set; }

        [DisplayName("Registration")]
        [Required(ErrorMessage = "You must provide a point of embarkation")]
        public string Embarkation { get; set; }

        [DisplayName("Registration")]
        [Required(ErrorMessage = "You must provide a destination")]
        public string Destination { get; set; }

        public virtual Airline Airline { get; set; }
    }
}
