using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IManufacturerManager
    {
        Task<Manufacturer> AddAsync(string name);
        Task<Manufacturer> GetAsync(Expression<Func<Manufacturer, bool>> predicate);
        IAsyncEnumerable<Manufacturer> ListAsync(Expression<Func<Manufacturer, bool>> predicate, int pageNumber, int pageSize);
        Task<int> CountAsync();
    }
}