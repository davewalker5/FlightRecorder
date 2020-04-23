using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FlightRecorder.Mvc.Entities
{
    public class Aircraft
    {
        public int Id { get; set; }

        [DisplayName("Model")]
        [Required(ErrorMessage = "You must provide a model")]
        public int ModelId { get; set; }

        [DisplayName("Registration")]
        [Required(ErrorMessage = "You must provide a registration number")]
        public string Registration { get; set; }

        [DisplayName("Serial Number")]
        [Required(ErrorMessage = "You must provide a serial number")]
        public string SerialNumber { get; set; }

        [DisplayName("Year of Manufacturer")]
        [Range(1, int.MaxValue, ErrorMessage = "You must provide a year of manufacture")]
        public int Manufactured { get; set; }

        public Model Model { get; set; }
    }
}
