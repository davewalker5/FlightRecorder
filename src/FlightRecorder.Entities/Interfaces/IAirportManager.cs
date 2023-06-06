using FlightRecorder.Entities.Db;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IAirportManager
    {
        Airport Add(string code, string name, string countryName);
        Task<Airport> AddAsync(string code, string name, string countryName);
        Airport Get(Expression<Func<Airport, bool>> predicate);
        Task<Airport> GetAsync(Expression<Func<Airport, bool>> predicate);
        IEnumerable<Airport> List(Expression<Func<Airport, bool>> predicate, int pageNumber, int pageSize);
        IAsyncEnumerable<Airport> ListAsync(Expression<Func<Airport, bool>> predicate, int pageNumber, int pageSize);
    }
}