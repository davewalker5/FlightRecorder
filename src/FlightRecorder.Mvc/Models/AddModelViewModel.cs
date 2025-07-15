namespace FlightRecorder.Mvc.Models
{
    public class AddModelViewModel : ModelViewModelBase
    {
        public string Message { get; set; }

        /// <summary>
        /// Reset the view model
        /// </summary>
        public void Clear()
        {
            Id = 0;
            ManufacturerId = 0;
            Name = "";
            Manufacturers = null;
            Message = "";
        }
    }
}
