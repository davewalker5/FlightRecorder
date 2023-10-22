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
        private readonly AirportsClient _client = new ServiceAccessor().GetService<AirportsClient>();

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            FlightDetailsViewModel model = (FlightDetailsViewModel)validationContext.ObjectInstance;
            List<string> codes = _client.GetAirportsAsync().Result.Select(x => x.Code).ToList();

            if ((validationContext.MemberName == "Embarkation") && !codes.Contains(model.Embarkation.ToUpper()))
            {
                return new ValidationResult($"{model.Embarkation} is not a valid IATA code for the point of embarkation");
            }

            if ((validationContext.MemberName == "Destination") && !codes.Contains(model.Destination.ToUpper()))
            {
                return new ValidationResult($"{model.Destination} is not a valid IATA code for the destination");
            }

            return ValidationResult.Success;
        }
    }
}
