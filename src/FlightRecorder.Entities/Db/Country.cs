using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public class Country
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
