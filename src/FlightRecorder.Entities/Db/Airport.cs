using FlightRecorder.Entities.DataExchange;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public class Airport
    {
        [Key]
        public long Id { get; set; }
        public long CountryId { get; set; }
        public string Code { get; set; }
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
    }
}
