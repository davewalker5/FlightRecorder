using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Api.Entities
{
    [ExcludeFromCodeCoverage]
    public record SightingStatisticsReport
    (
        int Aircraft,
        int Manufacturers,
        int Models,
        int Airlines,
        int Flights,
        int Sightings,
        int Locations
    );
}
