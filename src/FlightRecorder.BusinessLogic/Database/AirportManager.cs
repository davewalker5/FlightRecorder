using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FlightRecorder.BusinessLogic.Extensions;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlightRecorder.BusinessLogic.Database
{
    internal class AirportManager : IAirportManager
    {
        private readonly FlightRecorderFactory _factory;

        internal AirportManager(FlightRecorderFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Get the first airport matching the specified criteria along with the associated airline
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<Airport> GetAsync(Expression<Func<Airport, bool>> predicate)
        {
            List<Airport> airports = await ListAsync(predicate, 1, 1).ToListAsync();
            return airports.FirstOrDefault();
        }

        /// <summary>
        /// Get the airports matching the specified criteria along with the associated airlines
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IAsyncEnumerable<Airport> ListAsync(Expression<Func<Airport, bool>> predicate, int pageNumber, int pageSize)
        {
            IAsyncEnumerable<Airport> airports;

            if (predicate == null)
            {
                airports = _factory.Context.Airports
                                         .Include(m => m.Country)
                                         .OrderBy(a => a.Name)
                                         .Skip((pageNumber - 1) * pageSize)
                                         .Take(pageSize)
                                         .AsAsyncEnumerable();
            }
            else
            {
                airports = _factory.Context.Airports
                                         .Include(m => m.Country)
                                         .Where(predicate)
                                         .OrderBy(a => a.Name)
                                         .Skip((pageNumber - 1) * pageSize)
                                         .Take(pageSize)
                                         .AsAsyncEnumerable();
            }

            return airports;
        }

        /// <summary>
        /// Add a new airport
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="countryName"></param>
        /// <returns></returns>
        public async Task<Airport> AddAsync(string code, string name, string countryName)
        {
            code = code.CleanString().ToUpper();
            name = name.CleanString();
            countryName = countryName.CleanString();
            Airport airport = await GetAsync(a => (a.Code == code));

            if (airport == null)
            {
                Country country = await _factory.Countries.AddAsync(countryName);

                airport = new Airport
                {
                    Code = code,
                    Name = name,
                    CountryId = country.Id
                };

                await _factory.Context.Airports.AddAsync(airport);
                await _factory.Context.SaveChangesAsync();
                await _factory.Context.Entry(airport).Reference(m => m.Country).LoadAsync();
            }

            return airport;
        }
    }
}
