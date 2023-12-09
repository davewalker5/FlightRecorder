using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FlightRecorder.Mvc.Entities
{
    public class Airport
    {
        public int Id { get; set; }

        [DisplayName("Country")]
        [Required(ErrorMessage = "You must specify a country")]
        public int CountryId { get; set; }

        [DisplayName("IATA Code")]
        [Required(ErrorMessage = "You must specify an IATA Code")]
        public string Code { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "You must specify an airport name")]
        public string Name { get; set; }

        public virtual Country Country { get; set; }
    }
}
