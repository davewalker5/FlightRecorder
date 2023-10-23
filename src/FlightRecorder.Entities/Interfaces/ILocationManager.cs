using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Entities.Interfaces
{
    public interface ILocationManager
    {
        Task<Location> AddAsync(string name);
        Task<Location> GetAsync(Expression<Func<Location, bool>> predicate);
        IAsyncEnumerable<Location> ListAsync(Expression<Func<Location, bool>> predicate, int pageNumber, int pageSize);
    }
}