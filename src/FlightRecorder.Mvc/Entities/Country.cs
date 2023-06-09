using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace FlightRecorder.Mvc.Entities
{
    public class Country
    {
        public int Id { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "You must provide a name")]
        public string Name { get; set; }
    }
}
