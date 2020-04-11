using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.DataExchange;
using System.Threading.Tasks;

namespace FlightRecorder.Entities.Interfaces
{
    public interface ISightingManager
    {
        Sighting Add(long altitude, DateTime date, long locationId, long flightId, long aircraftId);
        Task<Sighting> AddAsync(long altitude, DateTime date, long locationId, long flightId, long aircraftId);
        Sighting Add(FlattenedSighting flattened);
        Task<Sighting> AddAsync(FlattenedSighting flattened);
        Sighting Get(Expression<Func<Sighting, bool>> predicate = null);
        Task<Sighting> GetAsync(Expression<Func<Sighting, bool>> predicate);
        IEnumerable<Sighting> List(Expression<Func<Sighting, bool>> predicate = null);
        IAsyncEnumerable<Sighting> ListAsync(Expression<Func<Sighting, bool>> predicate = null);
        IEnumerable<Sighting> ListByAircraft(string registration);
        Task<IAsyncEnumerable<Sighting>> ListByAircraftAsync(string registration);
        IEnumerable<Sighting> ListByRoute(string embarkation, string destination);
        Task<IAsyncEnumerable<Sighting>> ListByRouteAsync(string embarkation, string destination);
        IEnumerable<Sighting> ListByAirline(string airlineName);
        Task<IAsyncEnumerable<Sighting>> ListByAirlineAsync(string airlineName);
        IEnumerable<Sighting> ListByLocation(string locationName);
        Task<IAsyncEnumerable<Sighting>> ListByLocationAsync(string locationName);
    }
}