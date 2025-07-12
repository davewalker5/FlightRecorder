using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public class UserAttributeValue
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("User")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a user")]
        public int UserId { get; set; }

        [DisplayName("UserAttributeId")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a user attribute")]
        public int UserAttributeId { get; set; }

        public string Value { get; set; }

        public virtual UserAttribute UserAttribute { get; set; }
    }
}
