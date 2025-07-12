using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public partial class Aircraft
    {
        [Key]
        public long Id { get; set; }

        [DisplayName("Model")]
        public long? ModelId { get; set; }

        [DisplayName("Registration")]
        [Required(ErrorMessage = "You must provide a registration number")]
        public string Registration { get; set; }

        [DisplayName("Serial Number")]
        public string SerialNumber { get; set; }

        [DisplayName("Year of Manufacturer")]
        [Range(1, int.MaxValue, ErrorMessage = "Year of manufacture is not valid")]
        public long? Manufactured { get; set; }

        public virtual Model Model { get; set; }
    }
}
