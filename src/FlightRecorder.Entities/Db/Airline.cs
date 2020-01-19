using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FlightRecorder.Entities.Db
{
    public partial class Airline
    {
        public Airline()
        {
            Flight = new HashSet<Flight>();
        }

        [Key]
        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Flight> Flight { get; set; }
    }
}
