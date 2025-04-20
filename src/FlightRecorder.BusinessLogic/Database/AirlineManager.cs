using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FlightRecorder.BusinessLogic.Extensions;
using FlightRecorder.Data;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlightRecorder.BusinessLogic.Database
{
    internal class AirlineManager : IAirlineManager
    {
        private readonly FlightRecorderDbContext _context;

        internal AirlineManager(FlightRecorderDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get the first airline matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<Airline> GetAsync(Expression<Func<Airline, bool>> predicate)
        {
            List<Airline> airlines = await ListAsync(predicate, 1, 1).ToListAsync();
            return airlines.FirstOrDefault();
        }

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IAsyncEnumerable<Airline> ListAsync(Expression<Func<Airline, bool>> predicate, int pageNumber, int pageSize)
        {
            IAsyncEnumerable<Airline> results;
            if (predicate == null)
            {
                results = _context.Airlines
                                  .Skip((pageNumber - 1) * pageSize)
                                  .Take(pageSize)
                                  .AsAsyncEnumerable();
            }
            else
            {
                results = _context.Airlines.Where(predicate).AsAsyncEnumerable();
            }

            return results;
        }

        /// <summary>
        /// Return the number of airlines in the database
        /// </summary>
        /// <returns></returns>
        public async Task<int> CountAsync()
            => await _context.Airlines.CountAsync();

        /// <summary>
        /// Add a named airline, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Airline> AddAsync(string name)
        {
            name = name.CleanString();
            Airline airline = await GetAsync(a => a.Name == name);

            if (airline == null)
            {
                airline = new Airline { Name = name };
                await _context.Airlines.AddAsync(airline);
                await _context.SaveChangesAsync();
            }

            return airline;
        }
    }
}
