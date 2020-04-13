using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public partial class Model
    {
        public Model()
        {
            Aircraft = new HashSet<Aircraft>();
        }

        [Key]
        public long Id { get; set; }
        public long ManufacturerId { get; set; }
        public string Name { get; set; }

        public virtual Manufacturer Manufacturer { get; set; }
        public virtual ICollection<Aircraft> Aircraft { get; set; }
    }
}
