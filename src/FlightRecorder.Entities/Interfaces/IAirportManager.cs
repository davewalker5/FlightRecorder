using FlightRecorder.Entities.Db;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IAirportManager
    {
        Task<Airport> AddAsync(string code, string name, string countryName);
        Task<Airport> GetAsync(Expression<Func<Airport, bool>> predicate);
        IAsyncEnumerable<Airport> ListAsync(Expression<Func<Airport, bool>> predicate, int pageNumber, int pageSize);
    }
}