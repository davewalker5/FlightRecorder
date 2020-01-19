using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FlightRecorder.Entities.Db
{
    public partial class Manufacturer
    {
        public Manufacturer()
        {
            Model = new HashSet<Model>();
        }

        [Key]
        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Model> Model { get; set; }
    }
}
