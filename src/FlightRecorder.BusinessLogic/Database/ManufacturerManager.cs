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
    internal class ManufacturerManager : IManufacturerManager
    {
        private readonly FlightRecorderFactory _factory;

        internal ManufacturerManager(FlightRecorderFactory factory)
            => _factory = factory;

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
                results = _factory.Context.Manufacturers
                                  .OrderBy(m => m.Name)
                                  .Skip((pageNumber - 1) * pageSize)
                                  .Take(pageSize)
                                  .AsAsyncEnumerable();
            }
            else
            {
                results = _factory.Context.Manufacturers
                                  .Where(predicate)
                                  .OrderBy(m => m.Name)
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
            => await _factory.Context.Manufacturers.CountAsync();

        /// <summary>
        /// Add a named manufacturer, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Manufacturer> AddAsync(string name)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Adding manufacturer: Name = {name}");

            // Check the manufacturer doesn't already exist
            name = name.CleanString();            
            await CheckManufacturerIsNotADuplicate(name, 0);

            // Add the manufacturer and save changes
            var manufacturer = new Manufacturer { Name = name };
            await _factory.Context.Manufacturers.AddAsync(manufacturer);
            await _factory.Context.SaveChangesAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Added manufacturer {manufacturer}");

            return manufacturer;
        }

        /// <summary>
        /// Update a manufacturer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Manufacturer> UpdateAsync(long id, string name)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Updating manufacturer: ID = {id}, Name = {name}");

            // Retrieve the manufacturer
            var manufacturer = await _factory.Context.Manufacturers.FirstOrDefaultAsync(x => x.Id == id);
            if (manufacturer == null)
            {
                var message = $"Manufacturer with ID {id} not found";
                throw new ManufacturerNotFoundException(message);
            }

            // Check the manufacturer doesn't already exist
            name = name.CleanString();            
            await CheckManufacturerIsNotADuplicate(name, id);

            // Update the manufacturer properties and save changes
            manufacturer.Name = name;
            await _factory.Context.SaveChangesAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Updated manufacturer {manufacturer}");

            return manufacturer;
        }

        /// <summary>
        /// Delete the manufacturer with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="manufacturerNotFoundException"></exception>
        /// <exception cref="manufacturerInUseException"></exception>
        public async Task DeleteAsync(long id)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Deleting manufacturer: ID = {id}");

            // Check the manufacturer exists
            var manufacturer = await _factory.Context.Manufacturers.FirstOrDefaultAsync(x => x.Id == id);
            if (manufacturer == null)
            {
                var message = $"Manufacturer with ID {id} not found";
                throw new ManufacturerNotFoundException(message);
            }

            // Check there are no models for this manufacturer
            var models = await _factory.Models.ListAsync(x => x.ManufacturerId == id, 1, 1).ToListAsync();
            if (models.Any())
            {
                var message = $"Manufacturer with Id {id} has models associated with it and cannot be deleted";
                throw new ManufacturerInUseException(message);
            }

            // Remove the manufacturer
            _factory.Context.Remove(manufacturer);
            await _factory.Context.SaveChangesAsync();
        }

        /// <summary>
        /// Raise an exception if an attempt is made to add/update a manufacturer with a duplicate
        /// name
        /// </summary>
        /// <param code="name"></param>
        /// <param name="id"></param>
        /// <exception cref="ManufacturerExistsException"></exception>
        private async Task CheckManufacturerIsNotADuplicate(string name, long id)
        {
            var manufacturer = await _factory.Context.Manufacturers.FirstOrDefaultAsync(x => x.Name == name);
            if ((manufacturer != null) && (manufacturer.Id != id))
            {
                var message = $"Manufacturer {name} already exists";
                throw new ManufacturerExistsException(message);
            }
        }
    }
}
