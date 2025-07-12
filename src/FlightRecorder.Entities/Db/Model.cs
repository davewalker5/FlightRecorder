using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public partial class Model : FlightRecorderEntityBase
    {
        [Key]
        public long Id { get; set; }

        [DisplayName("Manufacturer")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a manufacturer")]
        public long ManufacturerId { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "You must provide a name")]
        public string Name { get; set; }

        public virtual Manufacturer Manufacturer { get; set; }
    }
}
