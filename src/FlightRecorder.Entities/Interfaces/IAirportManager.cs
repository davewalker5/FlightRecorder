using FlightRecorder.Entities.Db;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IAirportManager
    {
        Task<Airport> AddAsync(string code, string name, long countryId);
        Task<Airport> AddIfNotExistsAsync(string code, string name, long countryId);
        Task<Airport> GetAsync(Expression<Func<Airport, bool>> predicate);
        IAsyncEnumerable<Airport> ListAsync(Expression<Func<Airport, bool>> predicate, int pageNumber, int pageSize);
        Task<Airport> UpdateAsync(long id, string code, string name, long countryId);
        Task DeleteAsync(long id);
        Task CheckAirportExists(string code);
    }
}