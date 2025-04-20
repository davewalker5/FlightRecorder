using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public class UserAttributeValue
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int UserAttributeId { get; set; }
        public string Value { get; set; }

        public virtual UserAttribute UserAttribute { get; set; }
    }
}
