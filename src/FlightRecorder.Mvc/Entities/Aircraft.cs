using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FlightRecorder.Mvc.Entities
{
    public class Aircraft
    {
        public long Id { get; set; }

        [DisplayName("Model")]
        [Required(ErrorMessage = "You must provide a model")]
        public long ModelId { get; set; }

        [DisplayName("Registration")]
        [Required(ErrorMessage = "You must provide a registration number")]
        public string Registration { get; set; }

        [DisplayName("Serial Number")]
        [Required(ErrorMessage = "You must provide a serial number")]
        public string SerialNumber { get; set; }

        [DisplayName("Year of Manufacture")]
        [Required(ErrorMessage = "You must provide a year of manufacture")]
        public long Manufactured { get; set; }
    }
}
