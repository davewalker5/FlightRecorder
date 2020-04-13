using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public partial class Airline
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
