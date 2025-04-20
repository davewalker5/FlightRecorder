using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Api
{
    [ExcludeFromCodeCoverage]
    public class ApiPropertyDefinition
    {
        public ApiPropertyType PropertyType { get; set; }
        public string JsonPath { get; set; }
    }
}
