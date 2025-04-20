using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IModelManager
    {
        Task<Model> AddAsync(string name, string manufacturerName);
        Task<Model> GetAsync(Expression<Func<Model, bool>> predicate);
        IAsyncEnumerable<Model> ListAsync(Expression<Func<Model, bool>> predicate, int pageNumber, int pageSize);
        Task<int> CountAsync();
        IAsyncEnumerable<Model> ListByManufacturerAsync(string manufacturerName, int pageNumber, int pageSize);
    }
}