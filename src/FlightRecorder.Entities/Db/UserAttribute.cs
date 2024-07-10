using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public class UserAttribute
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
