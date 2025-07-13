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
    internal class LocationManager : ILocationManager
    {
        private readonly FlightRecorderFactory _factory;

        internal LocationManager(FlightRecorderFactory factory)
        {
            _factory = factory;
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
                results = _factory.Context.Locations
                                  .OrderBy(l => l.Name)
                                  .Skip((pageNumber - 1) * pageSize)
                                  .Take(pageSize)
                                  .AsAsyncEnumerable();
            }
            else
            {
                results = _factory.Context.Locations
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
            => await _factory.Context.Locations.CountAsync();

        /// <summary>
        /// Add a named location, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Location> AddAsync(string name)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Adding location: Name = {name}");

            // Check the location doesn't already exist
            name = name.CleanString();            
            await CheckLocationIsNotADuplicate(name, 0);

            // Add the location and save changes
            var location = new Location { Name = name };
            await _factory.Context.Locations.AddAsync(location);
            await _factory.Context.SaveChangesAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Added location {location}");

            return location;
        }

        /// <summary>
        /// Update a location
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Location> UpdateAsync(long id, string name)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Updating location: ID = {id}, Name = {name}");

            // Retrieve the location
            var location = await _factory.Context.Locations.FirstOrDefaultAsync(x => x.Id == id);
            if (location == null)
            {
                var message = $"Location with ID {id} not found";
                throw new LocationNotFoundException(message);
            }

            // Check the location doesn't already exist
            name = name.CleanString();            
            await CheckLocationIsNotADuplicate(name, id);

            // Update the location properties and save changes
            location.Name = name;
            await _factory.Context.SaveChangesAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Updated location {location}");

            return location;
        }

        /// <summary>
        /// Delete the location with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="locationNotFoundException"></exception>
        /// <exception cref="locationInUseException"></exception>
        public async Task DeleteAsync(long id)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Deleting location: ID = {id}");

            // Check the location exists
            var location = await _factory.Context.Locations.FirstOrDefaultAsync(x => x.Id == id);
            if (location == null)
            {
                var message = $"Location with ID {id} not found";
                throw new LocationNotFoundException(message);
            }

            // Check there are no sightings for this location
            var airports = await _factory.Sightings.ListAsync(x => x.LocationId == id, 1, 1).ToListAsync();
            if (airports.Any())
            {
                var message = $"Location with Id {id} has sightings associated with it and cannot be deleted";
                throw new LocationInUseException(message);
            }

            // Remove the location
            _factory.Context.Remove(location);
            await _factory.Context.SaveChangesAsync();
        }

        /// <summary>
        /// Raise an exception if an attempt is made to add/update a location with a duplicate
        /// name
        /// </summary>
        /// <param code="name"></param>
        /// <param name="id"></param>
        /// <exception cref="LocationExistsException"></exception>
        private async Task CheckLocationIsNotADuplicate(string name, long id)
        {
            var location = await _factory.Context.Locations.FirstOrDefaultAsync(x => x.Name == name);
            if ((location != null) && (location.Id != id))
            {
                var message = $"Location {name} already exists";
                throw new LocationExistsException(message);
            }
        }
    }
}
