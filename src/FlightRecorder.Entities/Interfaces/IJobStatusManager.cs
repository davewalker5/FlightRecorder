using FlightRecorder.Entities.Db;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IJobStatusManager
    {
        Task<JobStatus> AddAsync(string name, string parameters);
        Task<JobStatus> GetAsync(Expression<Func<JobStatus, bool>> predicate);
        IAsyncEnumerable<JobStatus> ListAsync(Expression<Func<JobStatus, bool>> predicate, int pageNumber, int pageSize);
        Task<JobStatus> UpdateAsync(long id, string error);
    }
}