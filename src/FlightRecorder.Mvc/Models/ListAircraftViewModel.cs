using System.Collections.Generic;
using FlightRecorder.Mvc.Entities;

namespace FlightRecorder.Mvc.Models
{
    public class ListAircraftViewModel : AircraftViewModelBase
    {
        public List<Aircraft> Aircraft { get; set; }
    }
}
