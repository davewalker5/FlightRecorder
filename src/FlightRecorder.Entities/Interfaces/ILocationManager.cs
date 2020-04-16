using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Entities.Interfaces
{
    public interface ILocationManager
    {
        Location Add(string name);
        Task<Location> AddAsync(string name);
        Location Get(Expression<Func<Location, bool>> predicate);
        Task<Location> GetAsync(Expression<Func<Location, bool>> predicate);
        IEnumerable<Location> List(Expression<Func<Location, bool>> predicate, int pageNumber, int pageSize);
        IAsyncEnumerable<Location> ListAsync(Expression<Func<Location, bool>> predicate, int pageNumber, int pageSize);
    }
}