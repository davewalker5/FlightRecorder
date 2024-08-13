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
    internal class ManufacturerManager : IManufacturerManager
    {
        private readonly FlightRecorderDbContext _context;

        internal ManufacturerManager(FlightRecorderDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Return the first entity matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<Manufacturer> GetAsync(Expression<Func<Manufacturer, bool>> predicate)
        {
            List<Manufacturer> manufacturers = await ListAsync(predicate, 1, 1).ToListAsync();
            return manufacturers.FirstOrDefault();
        }

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IAsyncEnumerable<Manufacturer> ListAsync(Expression<Func<Manufacturer, bool>> predicate, int pageNumber, int pageSize)
        {
            IAsyncEnumerable<Manufacturer> results;
            if (predicate == null)
            {
                results = _context.Manufacturers
                                  .Skip((pageNumber - 1) * pageSize)
                                  .Take(pageSize)
                                  .AsAsyncEnumerable();
            }
            else
            {
                results = _context.Manufacturers
                                  .Where(predicate)
                                  .Skip((pageNumber - 1) * pageSize)
                                  .Take(pageSize)
                                  .AsAsyncEnumerable();
            }

            return results;
        }

        /// <summary>
        /// Return the number of manufacturers in the database
        /// </summary>
        /// <returns></returns>
        public async Task<int> CountAsync()
            => await _context.Manufacturers.CountAsync();

        /// <summary>
        /// Add a named manufacturer, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Manufacturer> AddAsync(string name)
        {
            name = name.CleanString();
            Manufacturer manufacturer = await GetAsync(a => a.Name == name);

            if (manufacturer == null)
            {
                manufacturer = new Manufacturer { Name = name };
                await _context.Manufacturers.AddAsync(manufacturer);
                await _context.SaveChangesAsync();
            }

            return manufacturer;
        }
    }
}
