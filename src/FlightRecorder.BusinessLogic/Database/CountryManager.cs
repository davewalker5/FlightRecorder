using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FlightRecorder.BusinessLogic.Extensions;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Exceptions;
using FlightRecorder.Entities.Interfaces;
using FlightRecorder.Entities.Logging;
using Microsoft.EntityFrameworkCore;

namespace FlightRecorder.BusinessLogic.Database
{
    internal class CountryManager : ICountryManager
    {
        private readonly FlightRecorderFactory _factory;

        internal CountryManager(FlightRecorderFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Get the first airline matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<Country> GetAsync(Expression<Func<Country, bool>> predicate)
        {
            List<Country> countries = await ListAsync(predicate, 1, 1).ToListAsync();
            return countries.FirstOrDefault();
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
                results = _factory.Context.Countries
                                  .OrderBy(c => c.Name)
                                  .Skip((pageNumber - 1) * pageSize)
                                  .Take(pageSize)
                                  .AsAsyncEnumerable();
            }
            else
            {
                results = _factory.Context.Countries
                                  .Where(predicate)
                                  .OrderBy(c => c.Name)
                                  .AsAsyncEnumerable();
            }

            return results;
        }

        /// <summary>
        /// Add a named airline, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Country> AddAsync(string name)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Adding country: Name = {name}");

            // Check the country doesn't already exist
            name = name.CleanString();            
            await CheckCountryIsNotADuplicate(name, 0);

            // Add the country and save changes
            var country = new Country { Name = name };
            await _factory.Context.Countries.AddAsync(country);
            await _factory.Context.SaveChangesAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Added country {country}");

            return country;
        }

        /// <summary>
        /// Update an country
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Country> UpdateAsync(long id, string name)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Udating country: ID = {id}, Name = {name}");

            // Retrieve the country
            var country = await _factory.Context.Countries.FirstOrDefaultAsync(x => x.Id == id);
            if (country == null)
            {
                var message = $"Country with ID {id} not found";
                throw new CountryNotFoundException(message);
            }

            // Check the country doesn't already exist
            name = name.CleanString();            
            await CheckCountryIsNotADuplicate(name, id);

            // Update the country properties and save changes
            country.Name = name;
            await _factory.Context.SaveChangesAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Updated country {country}");

            return country;
        }

        /// <summary>
        /// Delete the country with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="CountryNotFoundException"></exception>
        /// <exception cref="CountryInUseException"></exception>
        public async Task DeleteAsync(long id)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Deleting country: ID = {id}");

            // Check the country exists
            var country = await _factory.Context.Countries.FirstOrDefaultAsync(x => x.Id == id);
            if (country == null)
            {
                var message = $"Country with ID {id} not found";
                throw new CountryNotFoundException(message);
            }

            // Check there are no airports for this country
            var airports = await _factory.Airports.ListAsync(x => x.CountryId == id, 1, 1).ToListAsync();
            if (airports.Any())
            {
                var message = $"Country with Id {id} has airports associated with it and cannot be deleted";
                throw new CountryInUseException(message);
            }

            // Remove the country
            _factory.Context.Remove(country);
            await _factory.Context.SaveChangesAsync();
        }

        /// <summary>
        /// Raise an exception if an attempt is made to add/update a country with a duplicate
        /// name
        /// </summary>
        /// <param code="name"></param>
        /// <param name="id"></param>
        /// <exception cref="CountryExistsException"></exception>
        private async Task CheckCountryIsNotADuplicate(string name, long id)
        {
            var Country = await _factory.Context.Countries.FirstOrDefaultAsync(x => x.Name == name);
            if ((Country != null) && (Country.Id != id))
            {
                var message = $"Country {name} already exists";
                throw new CountryExistsException(message);
            }
        }
    }
}
