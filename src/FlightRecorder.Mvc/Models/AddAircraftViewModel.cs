namespace FlightRecorder.Mvc.Models
{
    public class AddAircraftViewModel : AircraftViewModelBase
    {
        public string Message { get; set; }

        /// <summary>
        /// Reset the view model
        /// </summary>
        public void Clear()
        {
            Id = 0;
            ManufacturerId = 0;
            ModelId = 0;
            Registration = "";
            SerialNumber = "";
            Age = 0;
            Message = "";
        }
    }
}
