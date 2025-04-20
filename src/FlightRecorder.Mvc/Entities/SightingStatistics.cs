namespace FlightRecorder.Mvc.Entities
{
    public record SightingStatistics
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
