using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IAircraftManager
    {
        Task<Aircraft> AddAsync(string registration, string serialNumber, long? yearOfManufacture, string modelName, string manufacturerName);
        Task<Aircraft> GetAsync(Expression<Func<Aircraft, bool>> predicate);
        IAsyncEnumerable<Aircraft> ListAsync(Expression<Func<Aircraft, bool>> predicate, int pageNumber, int pageSize);
        Task<int> CountAsync();
        Task<IAsyncEnumerable<Aircraft>> ListByModelAsync(string modelName, int pageNumber, int pageSize);
        Task<IAsyncEnumerable<Aircraft>> ListByManufacturerAsync(string manufacturerName, int pageNumber, int pageSize);
    }
}