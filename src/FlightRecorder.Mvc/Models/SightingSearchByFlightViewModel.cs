using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FlightRecorder.Mvc.Models
{
    public class SightingSearchByFlightViewModel : SightingsSearchViewModelBase
    {
        [DisplayName("Flight number")]
        [Required(ErrorMessage = "You must provide a flight number")]
        public string FlightNumber { get; set; }
    }
}
