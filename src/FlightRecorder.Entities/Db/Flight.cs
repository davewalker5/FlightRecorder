using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public partial class Flight
    {
        [Key]
        public long Id { get; set; }
        public long AirlineId { get; set; }
        public string Number { get; set; }
        public string Embarkation { get; set; }
        public string Destination { get; set; }

        public virtual Airline Airline { get; set; }
    }
}
