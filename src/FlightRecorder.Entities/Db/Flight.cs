using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FlightRecorder.Entities.Db
{
    public partial class Flight
    {
        public Flight()
        {
            Sighting = new HashSet<Sighting>();
        }

        [Key]
        public long Id { get; set; }
        public long AirlineId { get; set; }
        public string Number { get; set; }
        public string Embarkation { get; set; }
        public string Destination { get; set; }

        public virtual Airline Airline { get; set; }
        public virtual ICollection<Sighting> Sighting { get; set; }
    }
}
