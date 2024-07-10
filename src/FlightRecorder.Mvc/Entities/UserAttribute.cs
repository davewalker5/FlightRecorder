using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FlightRecorder.Mvc.Entities
{
    public class UserAttribute
    {
        public int Id { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "You must provide a user attribute name")]
        public string Name { get; set; }
    }
}
