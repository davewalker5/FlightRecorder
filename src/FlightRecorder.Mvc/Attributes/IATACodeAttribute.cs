using FlightRecorder.Mvc.Api;
using System.ComponentModel.DataAnnotations;

namespace FlightRecorder.Mvc.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class IATACodeAttribute : ValidationAttribute
    {
        private readonly AirportsClient _client = new ServiceAccessor().GetService<AirportsClient>();

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Get a list of airport IATA codes to validate against
            var codes = _client.GetAirportsAsync(1, int.MaxValue).Result.Select(x => x.Code).ToList();

            // Get the properties of the object instance being validated
            var properties = validationContext.ObjectInstance.GetType().GetProperties();

            // See if the object has the named property (at this point, it should have)
            var property = properties.FirstOrDefault(x => x.Name == validationContext.MemberName);
            if (property != null)
            {
                // It does, so get the value and check it's in the code list
                var propertyValue = ((string)property.GetValue(validationContext.ObjectInstance) ?? "").ToUpper();
                if (!codes.Contains(propertyValue))
                {
                    return new ValidationResult($"{propertyValue} is not a valid IATA code for {validationContext.MemberName}");
                }
            }

            return ValidationResult.Success;
        }
    }
}
