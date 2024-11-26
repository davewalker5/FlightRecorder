using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FlightRecorder.Mvc.Entities
{
    public class Aircraft
    {
        public int Id { get; set; }

        [DisplayName("Model")]
        public int? ModelId { get; set; }

        [DisplayName("Registration")]
        [Required(ErrorMessage = "You must provide a registration number")]
        public string Registration { get; set; }

        [DisplayName("Serial Number")]
        public string SerialNumber { get; set; }

        [DisplayName("Year of Manufacturer")]
        [Range(1, int.MaxValue, ErrorMessage = "Year of manufacture is not valid")]
        public int? Manufactured { get; set; }

        public Model Model { get; set; }
    }
}
