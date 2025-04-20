using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FlightRecorder.Mvc.Entities
{
    public class Model
    {
        public int Id { get; set; }

        [DisplayName("Manufacturer")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a manufacturer")]
        public int ManufacturerId { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "You must provide a name")]
        public string Name { get; set; }

        public Manufacturer Manufacturer { get; set; }
    }
}
