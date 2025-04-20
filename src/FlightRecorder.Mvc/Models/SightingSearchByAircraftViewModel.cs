using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FlightRecorder.Mvc.Models
{
    public class SightingSearchByAircraftViewModel : SightingsSearchViewModelBase
    {
        [DisplayName("Registration")]
        [Required(ErrorMessage = "You must provide an aircraft registration number")]
        public string Registration { get; set; }
    }
}
