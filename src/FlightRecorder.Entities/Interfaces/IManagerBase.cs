using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IManagerBase<T> where T : class
    {
        T Add(T entity);
        Task<T> AddAsync(T entity);
        T Get(Expression<Func<T, bool>> predicate = null);
        Task<T> GetAsync(Expression<Func<T, bool>> predicate = null);
        IEnumerable<T> List(Expression<Func<T, bool>> predicate = null);
        IAsyncEnumerable<T> ListAsync(Expression<Func<T, bool>> predicate = null);
    }
}