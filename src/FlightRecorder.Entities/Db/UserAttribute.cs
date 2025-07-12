using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public class UserAttribute : FlightRecorderEntityBase
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "You must provide a user attribute name")]
        public string Name { get; set; }
    }
}
