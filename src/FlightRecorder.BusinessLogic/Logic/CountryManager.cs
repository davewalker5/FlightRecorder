using FlightRecorder.BusinessLogic.Extensions;
using FlightRecorder.Data;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace FlightRecorder.BusinessLogic.Logic
{
    public class CountryManager : ICountryManager
    {
        private readonly FlightRecorderDbContext _context;

        internal CountryManager(FlightRecorderDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Return the first entity matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Country Get(Expression<Func<Country, bool>> predicate)
            => List(predicate, 1, 1).FirstOrDefault();

        /// <summary>
        /// Get the first airline matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<Country> GetAsync(Expression<Func<Country, bool>> predicate)
        {
            List<Country> countries = await _context.Countries
                                                    .Where(predicate)
                                                    .ToListAsync();
            return countries.FirstOrDefault();
        }

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<Country> List(Expression<Func<Country, bool>> predicate, int pageNumber, int pageSize)
        {
            IEnumerable<Country> results;
            if (predicate == null)
            {
                results = _context.Countries;
                results = results.Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize);
            }
            else
            {
                results = _context.Countries
                                  .Where(predicate)
                                  .Skip((pageNumber - 1) * pageSize)
                                  .Take(pageSize);
            }

            return results;
        }

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IAsyncEnumerable<Country> ListAsync(Expression<Func<Country, bool>> predicate, int pageNumber, int pageSize)
        {
            IAsyncEnumerable<Country> results;
            if (predicate == null)
            {
                results = _context.Countries
                                  .Skip((pageNumber - 1) * pageSize)
                                  .Take(pageSize)
                                  .AsAsyncEnumerable();
            }
            else
            {
                results = _context.Countries.Where(predicate).AsAsyncEnumerable();
            }

            return results;
        }

        /// <summary>
        /// Add a named country, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Country Add(string name)
        {
            name = name.CleanString();
            Country country = Get(a => a.Name == name);

            if (country == null)
            {
                country = new Country { Name = name };
                _context.Countries.Add(country);
                _context.SaveChanges();
            }

            return country;
        }

        /// <summary>
        /// Add a named airline, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Country> AddAsync(string name)
        {
            name = name.CleanString();
            Country country = await GetAsync(a => a.Name == name);

            if (country == null)
            {
                country = new Country { Name = name };
                await _context.Countries.AddAsync(country);
                await _context.SaveChangesAsync();
            }

            return country;
        }
    }
}
