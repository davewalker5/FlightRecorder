using FlightRecorder.Entities.Db;

namespace FlightRecorder.Mvc.Models
{
    public class AddLocationViewModel : Location
    {
        public string Message { get; set; }

        public void Clear()
        {
            Id = 0;
            Name = "";
            Message = "";
        }
    }
}