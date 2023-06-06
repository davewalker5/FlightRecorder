using FlightRecorder.Entities.Db;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FlightRecorder.Entities.Interfaces
{
    public interface ICountryManager
    {
        Country Add(string name);
        Task<Country> AddAsync(string name);
        Country Get(Expression<Func<Country, bool>> predicate);
        Task<Country> GetAsync(Expression<Func<Country, bool>> predicate);
        IEnumerable<Country> List(Expression<Func<Country, bool>> predicate, int pageNumber, int pageSize);
        IAsyncEnumerable<Country> ListAsync(Expression<Func<Country, bool>> predicate, int pageNumber, int pageSize);
    }
}