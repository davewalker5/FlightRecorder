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
        Task<Sighting> AddAsync(long altitude, DateTime date, long locationId, long flightId, long aircraftId);
        Task<Sighting> AddAsync(FlattenedSighting flattened);
        Task<Sighting> GetAsync(Expression<Func<Sighting, bool>> predicate);
        IAsyncEnumerable<Sighting> ListAsync(Expression<Func<Sighting, bool>> predicate, int pageNumber, int pageSize);
        Task<IAsyncEnumerable<Sighting>> ListByAircraftAsync(string registration, int pageNumber, int pageSize);
        Task<IAsyncEnumerable<Sighting>> ListByRouteAsync(string embarkation, string destination, int pageNumber, int pageSize);
        Task<IAsyncEnumerable<Sighting>> ListByAirlineAsync(string airlineName, int pageNumber, int pageSize);
        Task<IAsyncEnumerable<Sighting>> ListByLocationAsync(string locationName, int pageNumber, int pageSize);
    }
}