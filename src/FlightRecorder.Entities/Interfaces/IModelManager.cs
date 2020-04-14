using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IModelManager
    {
        Model Add(string name, string manufacturerName);
        Task<Model> AddAsync(string name, string manufacturerName);
        Model Get(Expression<Func<Model, bool>> predicate);
        Task<Model> GetAsync(Expression<Func<Model, bool>> predicate);
        IEnumerable<Model> List(Expression<Func<Model, bool>> predicate, int pageNumber, int pageSize);
        IAsyncEnumerable<Model> ListAsync(Expression<Func<Model, bool>> predicate, int pageNumber, int pageSize);
        IEnumerable<Model> ListByManufacturer(string manufacturerName, int pageNumber, int pageSize);
        IAsyncEnumerable<Model> ListByManufacturerAsync(string manufacturerName, int pageNumber, int pageSize);
    }
}