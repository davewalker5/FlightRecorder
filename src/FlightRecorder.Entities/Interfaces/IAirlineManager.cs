using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IAirlineManager
    {
        Task<Airline> AddAsync(string name);
        Task<Airline> GetAsync(Expression<Func<Airline, bool>> predicate);
        IAsyncEnumerable<Airline> ListAsync(Expression<Func<Airline, bool>> predicate, int pageNumber, int pageSize);
        Task<int> CountAsync();
        Task<Airline> UpdateAsync(int id, string name);
        Task DeleteAsync(long id);
    }
}