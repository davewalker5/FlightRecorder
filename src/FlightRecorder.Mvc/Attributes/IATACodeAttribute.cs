using FlightRecorder.Mvc.Api;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FlightRecorder.Mvc.Models;

namespace FlightRecorder.Mvc.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class IATACodeAttribute : ValidationAttribute
    {
        private readonly List<string> _codes;

        public IATACodeAttribute()
        {
            AirportsClient client = new ServiceAccessor().GetService<AirportsClient>();
            _codes = client.GetAirportsAsync().Result.Select(x => x.Code).ToList();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            FlightDetailsViewModel model = (FlightDetailsViewModel)validationContext.ObjectInstance;

            if ((validationContext.MemberName == "Embarkation") && !_codes.Contains(model.Embarkation))
            {
                return new ValidationResult($"{model.Embarkation} is not a valid IATA code for the point of embarkation");
            }

            if ((validationContext.MemberName == "Destination") && !_codes.Contains(model.Destination))
            {
                return new ValidationResult($"{model.Destination} is not a valid IATA code for the destination");
            }

            return ValidationResult.Success;
        }
    }
}
