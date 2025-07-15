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
    internal class AirlineManager : IAirlineManager
    {
        private readonly FlightRecorderFactory _factory;

        internal AirlineManager(FlightRecorderFactory factory)
        {
            _factory = factory;
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
                results = _factory.Context.Airlines
                                  .OrderBy(a => a.Name)
                                  .Skip((pageNumber - 1) * pageSize)
                                  .Take(pageSize)
                                  .AsAsyncEnumerable();
            }
            else
            {
                results = _factory.Context.Airlines
                                  .Where(predicate)
                                  .OrderBy(a => a.Name)
                                  .AsAsyncEnumerable();
            }

            return results;
        }

        /// <summary>
        /// Return the number of airlines in the database
        /// </summary>
        /// <returns></returns>
        public async Task<int> CountAsync()
            => await _factory.Context.Airlines.CountAsync();

        /// <summary>
        /// Add a named airline, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Airline> AddAsync(string name)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Adding airline: Name = {name}");

            // Check the airline doesn't already exist
            name = name.CleanString();            
            await CheckAirlineIsNotADuplicate(name, 0);

            // Add the airline and save changes
            var airline = new Airline { Name = name };
            await _factory.Context.Airlines.AddAsync(airline);
            await _factory.Context.SaveChangesAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Added airline {airline}");

            return airline;
        }

        /// <summary>
        /// Add a named airline, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Airline> AddIfNotExistsAsync(string name)
        {
            name = name.CleanString();
            var airline = await GetAsync(x => x.Name == name);
            if (airline == null)
            {
                airline = await AddAsync(name);
            }
            return airline;
        }

        /// <summary>
        /// Update an airline
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Airline> UpdateAsync(long id, string name)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Updating airline: ID = {id}, Name = {name}");

            // Retrieve the airline
            var airline = await _factory.Context.Airlines.FirstOrDefaultAsync(x => x.Id == id);
            if (airline == null)
            {
                var message = $"Airline with ID {id} not found";
                throw new AirlineNotFoundException(message);
            }

            // Check we're not going to create a duplicate
            name = name.CleanString();            
            await CheckAirlineIsNotADuplicate(name, id);

            // Update the airline properties and save changes
            airline.Name = name;
            await _factory.Context.SaveChangesAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Updated airline {airline}");

            return airline;
        }

        /// <summary>
        /// Delete the airline with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="AirlineNotFoundException"></exception>
        /// <exception cref="AirlineInUseException"></exception>
        public async Task DeleteAsync(long id)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Deleting airline: ID = {id}");

            // Check the airline exists
            var airline = await _factory.Context.Airlines.FirstOrDefaultAsync(x => x.Id == id);
            if (airline == null)
            {
                var message = $"Airline with ID {id} not found";
                throw new AirlineNotFoundException(message);
            }

            // Check there are no flights for this airline
            var flights = await _factory.Flights.ListAsync(x => x.AirlineId == id, 1, 1).ToListAsync();
            if (flights.Any())
            {
                var message = $"Airline with Id {id} has flights associated with it and cannot be deleted";
                throw new AirlineInUseException(message);
            }

            // Remove the airline
            _factory.Context.Remove(airline);
            await _factory.Context.SaveChangesAsync();
        }

        /// <summary>
        /// Raise an exception if the specified airline doesn't exist
        /// </summary>
        /// <param name="airlineId"></param>
        /// <exception cref="AirlineNotFoundException"></exception>
        public async Task CheckAirlineExists(long airlineId)
        {
            var airline = await _factory.Airlines.GetAsync(x => x.Id == airlineId);
            if (airline == null)
            {
                var message = $"Airline with ID {airlineId} not found";
                throw new AirlineNotFoundException(message);
            }
        }

        /// <summary>
        /// Raise an exception if an attempt is made to add/update an airline with a duplicate
        /// name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <exception cref="AirlineExistsException"></exception>
        private async Task CheckAirlineIsNotADuplicate(string name, long id)
        {
            var airline = await _factory.Context.Airlines.FirstOrDefaultAsync(x => x.Name == name);
            if ((airline != null) && (airline.Id != id))
            {
                var message = $"Airline {name} already exists";
                throw new AirlineExistsException(message);
            }
        }
    }
}
