using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.DataExchange;

namespace FlightRecorder.Entities.Interfaces
{
    public interface ISightingManager
    {
        Sighting Add(long altitude, DateTime date, long locationId, long flightId, long aircraftId);
        Sighting Add(FlattenedSighting flattened);
        Sighting Get(Expression<Func<Sighting, bool>> predicate = null);
        IEnumerable<Sighting> List(Expression<Func<Sighting, bool>> predicate = null);
        IEnumerable<Sighting> ListByAircraft(string registration);
        IEnumerable<Sighting> ListByRoute(string embarkation, string destination);
        IEnumerable<Sighting> ListByAirline(string airlineName);
        IEnumerable<Sighting> ListByLocation(string locationName);
    }
}