using FlightRecorder.BusinessLogic.Extensions;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Data;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Exceptions;
using FlightRecorder.Entities.Interfaces;
using FlightRecorder.Entities.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FlightRecorder.BusinessLogic.Database
{
    internal class JobStatusManager : IJobStatusManager
    {
        private readonly FlightRecorderFactory _factory;

        internal JobStatusManager(FlightRecorderFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Get the first job status matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<JobStatus> GetAsync(Expression<Func<JobStatus, bool>> predicate)
        {
            List<JobStatus> statuses = await ListAsync(predicate, 1, 1).ToListAsync();
            return statuses.FirstOrDefault();
        }

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IAsyncEnumerable<JobStatus> ListAsync(Expression<Func<JobStatus, bool>> predicate, int pageNumber, int pageSize)
        {
            IAsyncEnumerable<JobStatus> results;
            if (predicate == null)
            {
                results = _factory.Context.JobStatuses
                                  .OrderByDescending(x => x.Start)
                                  .Skip((pageNumber - 1) * pageSize)
                                  .Take(pageSize)
                                  .AsAsyncEnumerable();
            }
            else
            {
                results = _factory.Context.JobStatuses
                                  .Where(predicate)
                                  .OrderByDescending(x => x.Start)
                                  .Skip((pageNumber - 1) * pageSize)
                                  .Take(pageSize)
                                  .AsAsyncEnumerable();
            }

            return results;
        }

        /// <summary>
        /// Create a new job status
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<JobStatus> AddAsync(string name, string parameters)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Adding job status: Name = {name}, Parameters = {parameters}");

            JobStatus status = new JobStatus
            {
                Name = name.CleanString(),
                Parameters = parameters.CleanString(),
                Start = DateTime.Now
            };

            await _factory.Context.JobStatuses.AddAsync(status);
            await _factory.Context.SaveChangesAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Added job status {status}");

            return status;
        }

        /// <summary>
        /// Update a job status, setting the end timestamp and error result
        /// </summary>
        /// <param name="id"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public async Task<JobStatus> UpdateAsync(long id, string error)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Updating job status: ID = {id}, Error = {error}");

            // Retrieve the job status record
            JobStatus status = await GetAsync(x => x.Id == id);
            if (status == null)
            {
                var message = $"Job status record with ID {id} not found";
                throw new JobStatusNotFoundException(message);
            }
            
            // Update the record properties and save changes
            status.End = DateTime.Now;
            status.Error = error.CleanString();
            await _factory.Context.SaveChangesAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Updated job status {status}");

            return status;
        }
    }
}
