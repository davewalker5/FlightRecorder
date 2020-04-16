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
        IEnumerable<Sighting> List(Expression<Func<Sighting, bool>> predicate, int pageNumber, int pageSize);
        IAsyncEnumerable<Sighting> ListAsync(Expression<Func<Sighting, bool>> predicate, int pageNumber, int pageSize);
        IEnumerable<Sighting> ListByAircraft(string registration, int pageNumber, int pageSize);
        Task<IAsyncEnumerable<Sighting>> ListByAircraftAsync(string registration, int pageNumber, int pageSize);
        IEnumerable<Sighting> ListByRoute(string embarkation, string destination, int pageNumber, int pageSize);
        Task<IAsyncEnumerable<Sighting>> ListByRouteAsync(string embarkation, string destination, int pageNumber, int pageSize);
        IEnumerable<Sighting> ListByAirline(string airlineName, int pageNumber, int pageSize);
        Task<IAsyncEnumerable<Sighting>> ListByAirlineAsync(string airlineName, int pageNumber, int pageSize);
        IEnumerable<Sighting> ListByLocation(string locationName, int pageNumber, int pageSize);
        Task<IAsyncEnumerable<Sighting>> ListByLocationAsync(string locationName, int pageNumber, int pageSize);
    }
}