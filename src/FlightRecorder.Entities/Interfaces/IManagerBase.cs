using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IManagerBase<T> where T : class
    {
        T Add(T entity);
        T Get(Expression<Func<T, bool>> predicate = null);
        IEnumerable<T> List(Expression<Func<T, bool>> predicate = null);
    }
}