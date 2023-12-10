namespace FlightRecorder.Mvc.Models
{
    public class AddFlightViewModel : FlightViewModelBase
    {
        public string Message { get; set; }

        /// <summary>
        /// Reset the view model
        /// </summary>
        public void Clear()
        {
            Id = 0;
            AirlineId = 0;
            FlightNumber = "";
            Embarkation = "";
            Destination = "";
            Message = "";
        }
    }
}
