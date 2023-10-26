using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Api.Entities
{
    [ExcludeFromCodeCoverage]
    public class BackgroundWorkItem
    {
        public string JobName { get; set; }

        public override string ToString()
        {
            return $"JobName = {JobName}";
        }
    }
}
