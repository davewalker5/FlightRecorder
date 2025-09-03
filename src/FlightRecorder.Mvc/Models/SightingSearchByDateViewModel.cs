using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FlightRecorder.Mvc.Models
{
    public class SightingSearchByDateViewModel : SightingsSearchViewModelBase
    {
        [DisplayName("From")]
        [UIHint("DatePicker")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "You must enter a 'from' date")]
        public DateTime? From { get; set; }

        [DisplayName("To")]
        [UIHint("DatePicker")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "You must enter a 'to' date")]
        public DateTime? To { get; set; }

    }
}
