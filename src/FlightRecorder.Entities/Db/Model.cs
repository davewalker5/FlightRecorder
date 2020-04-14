using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public partial class Model
    {
        [Key]
        public long Id { get; set; }
        public long ManufacturerId { get; set; }
        public string Name { get; set; }

        public virtual Manufacturer Manufacturer { get; set; }
    }
}
