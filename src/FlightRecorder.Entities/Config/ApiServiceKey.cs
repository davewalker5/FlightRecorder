using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Config
{
    [ExcludeFromCodeCoverage]
    public class ApiServiceKey
    {
        public ApiServiceType Service { get; set; }
        public string Key { get; set; } = "";
    }
}
