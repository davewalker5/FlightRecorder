using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IAircraftManager
    {
        Aircraft Add(string registration, string serialNumber, long yearOfManufacture, string modelName, string manufacturerName);
        Task<Aircraft> AddAsync(string registration, string serialNumber, long yearOfManufacture, string modelName, string manufacturerName);
        Aircraft Get(Expression<Func<Aircraft, bool>> predicate = null);
        Task<Aircraft> GetAsync(Expression<Func<Aircraft, bool>> predicate);
        IEnumerable<Aircraft> List(Expression<Func<Aircraft, bool>> predicate = null);
        IAsyncEnumerable<Aircraft> ListAsync(Expression<Func<Aircraft, bool>> predicate = null);
        IEnumerable<Aircraft> ListByModel(string modelName);
        Task<IAsyncEnumerable<Aircraft>> ListByModelAsync(string modelName);
        IEnumerable<Aircraft> ListByManufacturer(string manufacturerName);
        Task<IAsyncEnumerable<Aircraft>> ListByManufacturerAsync(string manufacturerName);
    }
}