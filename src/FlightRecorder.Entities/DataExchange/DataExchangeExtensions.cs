using System.Collections.Generic;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Entities.DataExchange
{
    public static class DataExchangeExtensions
    {
        /// <summary>
        /// Return a collection of flattened sightings from a collection of sightings
        /// </summary>
        /// <param name="sightings"></param>
        /// <returns></returns>
        public static IEnumerable<FlattenedSighting> Flatten(this IEnumerable<Sighting> sightings)
        {
            List<FlattenedSighting> flattened = new List<FlattenedSighting>();

            foreach (Sighting sighting in sightings)
            {
                flattened.Add(sighting.Flatten());
            }

            return flattened;
        }

        /// <summary>
        /// Return a collection of flattened airports from a collection of airports
        /// </summary>
        /// <param name="sightings"></param>
        /// <returns></returns>
        public static IEnumerable<FlattenedAirport> Flatten(this IEnumerable<Airport> airports)
        {
            List<FlattenedAirport> flattened = new List<FlattenedAirport>();

            foreach (Airport airport in airports)
            {
                flattened.Add(airport.Flatten());
            }

            return flattened;
        }
    }
}
