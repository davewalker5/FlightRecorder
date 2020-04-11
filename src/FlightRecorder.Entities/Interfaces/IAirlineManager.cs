using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IAirlineManager
    {
        Airline Add(string name);
        Task<Airline> AddAsync(string name);
        Airline Get(Expression<Func<Airline, bool>> predicate);
        Task<Airline> GetAsync(Expression<Func<Airline, bool>> predicate);
        IEnumerable<Airline> List(Expression<Func<Airline, bool>> predicate = null);
        IAsyncEnumerable<Airline> ListAsync(Expression<Func<Airline, bool>> predicate = null);
    }
}