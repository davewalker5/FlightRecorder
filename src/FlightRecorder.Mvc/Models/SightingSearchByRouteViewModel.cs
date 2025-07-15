using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FlightRecorder.Mvc.Models
{
    public class SightingSearchByRouteViewModel : SightingsSearchViewModelBase
    {
        [DisplayName("Point of Embarkation")]
        [Required(ErrorMessage = "You must provide a point of embarkation")]
        public string Embarkation { get; set; }

        [DisplayName("Destination Airport")]
        [Required(ErrorMessage = "You must provide a destination airport")]
        public string Destination { get; set; }

    }
}
