using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FlightRecorder.Mvc.Entities;

namespace FlightRecorder.Mvc.Models
{
    public class ListAircraftViewModel : AircraftViewModelBase
    {
        [DisplayName("Manufacturer")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a manufacturer")]
        public int ManufacturerId { get; set; }

        public List<Aircraft> Aircraft { get; set; }
    }
}
