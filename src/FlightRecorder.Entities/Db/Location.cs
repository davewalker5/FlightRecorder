using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public partial class Location
    {
        public Location()
        {
            Sighting = new HashSet<Sighting>();
        }

        [Key]
        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Sighting> Sighting { get; set; }
    }
}
