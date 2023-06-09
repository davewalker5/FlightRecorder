using FlightRecorder.Mvc.Entities;

namespace FlightRecorder.Mvc.Models
{
    public class AddAirportViewModel : AirportViewModelBase
    {
        public string Message { get; set; }

        public void Clear()
        {
            Id = 0;
            CountryId = 0;
            Name = "";
            Message = "";
        }
    }
}
