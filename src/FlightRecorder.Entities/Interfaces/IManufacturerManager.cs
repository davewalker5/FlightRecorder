using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IManufacturerManager
    {
        Manufacturer Add(string name);
        Task<Manufacturer> AddAsync(string name);
        Manufacturer Get(Expression<Func<Manufacturer, bool>> predicate);
        Task<Manufacturer> GetAsync(Expression<Func<Manufacturer, bool>> predicate);
        IEnumerable<Manufacturer> List(Expression<Func<Manufacturer, bool>> predicate, int pageNumber, int pageSize);
        IAsyncEnumerable<Manufacturer> ListAsync(Expression<Func<Manufacturer, bool>> predicate, int pageNumber, int pageSize);
    }
}