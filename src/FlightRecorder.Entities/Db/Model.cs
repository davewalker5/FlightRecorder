using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FlightRecorder.Entities.Db
{
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
