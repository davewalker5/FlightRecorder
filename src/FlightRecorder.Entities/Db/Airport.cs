using FlightRecorder.Entities.DataExchange;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public class Airport : FlightRecorderEntityBase
    {
        [Key]
        public long Id { get; set; }

        [DisplayName("Country")]
        [Required(ErrorMessage = "You must specify a country")]
        public long CountryId { get; set; }

        [DisplayName("IATA Code")]
        [Required(ErrorMessage = "You must specify an IATA Code")]
        public string Code { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "You must specify an airport name")]
        public string Name { get; set; }

        public virtual Country Country { get; set; }

        public FlattenedAirport Flatten()
        {
            return new FlattenedAirport
            {
                Code = Code,
                Name = Name,
                Country = Country.Name
            };
        }

        public string Description()
            => $"{Code} - {Name}, {Country}";
    }
}
