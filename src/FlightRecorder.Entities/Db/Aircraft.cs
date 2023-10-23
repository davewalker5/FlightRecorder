using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public partial class Aircraft
    {
        [Key]
        public long Id { get; set; }
        public long? ModelId { get; set; }
        public string Registration { get; set; }
        public string SerialNumber { get; set; }
        public long? Manufactured { get; set; }

        public virtual Model Model { get; set; }
    }
}
