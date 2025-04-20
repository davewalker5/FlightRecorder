using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FlightRecorder.Mvc.Entities
{
    public class UserAttributeValue
    {
        public int Id { get; set; }

        [DisplayName("User")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a user")]
        public int UserId { get; set; }

        [DisplayName("UserAttributeId")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a user attribute")]
        public int UserAttributeId { get; set; }

        public string Value { get; set; }

        public UserAttribute UserAttribute { get; set; }
    }
}
