using System.Collections.Generic;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Mvc.Models
{
    public class ListAircraftViewModel : AircraftViewModelBase
    {
        public List<Aircraft> Aircraft { get; set; }
    }
}
