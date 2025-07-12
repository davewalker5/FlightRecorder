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
    internal class LocationManager : ILocationManager
    {
        private readonly FlightRecorderDbContext _context;

        internal LocationManager(FlightRecorderDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Return the first entity matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<Location> GetAsync(Expression<Func<Location, bool>> predicate)
        {
            List<Location> locations = await ListAsync(predicate, 1, 1).ToListAsync();
            return locations.FirstOrDefault();
        }

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IAsyncEnumerable<Location> ListAsync(Expression<Func<Location, bool>> predicate, int pageNumber, int pageSize)
        {
            IAsyncEnumerable<Location> results;
            if (predicate == null)
            {
                results = _context.Locations
                                  .OrderBy(l => l.Name)
                                  .Skip((pageNumber - 1) * pageSize)
                                  .Take(pageSize)
                                  .AsAsyncEnumerable();
            }
            else
            {
                results = _context.Locations
                                  .Where(predicate)
                                  .OrderBy(l => l.Name)
                                  .Skip((pageNumber - 1) * pageSize)
                                  .Take(pageSize)
                                  .AsAsyncEnumerable();
            }

            return results;
        }

        /// <summary>
        /// Return the number of locations in the database
        /// </summary>
        /// <returns></returns>
        public async Task<int> CountAsync()
            => await _context.Locations.CountAsync();

        /// <summary>
        /// Add a named location, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Location> AddAsync(string name)
        {
            name = name.CleanString();
            Location location = await GetAsync(a => a.Name == name);

            if (location == null)
            {
                location = new Location { Name = name };
                await _context.Locations.AddAsync(location);
                await _context.SaveChangesAsync();
            }

            return location;
        }
    }
}
