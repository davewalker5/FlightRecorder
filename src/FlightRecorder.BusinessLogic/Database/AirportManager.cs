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
    internal class AirportManager : IAirportManager
    {
        private readonly FlightRecorderFactory _factory;

        internal AirportManager(FlightRecorderFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Get the first airport matching the specified criteria along with the associated airport
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<Airport> GetAsync(Expression<Func<Airport, bool>> predicate)
        {
            List<Airport> airports = await ListAsync(predicate, 1, 1).ToListAsync();
            return airports.FirstOrDefault();
        }

        /// <summary>
        /// Get the airports matching the specified criteria along with the associated airports
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
        /// <param name="countryId"></param>
        /// <returns></returns>
        public async Task<Airport> AddAsync(string code, string name, long countryId)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Adding airport: IATA Code = {code}, Name = {name}, Country ID = {countryId}");

            // Check the airport doesn't already exist
            code = code.CleanString().ToUpper();            
            await CheckAirportIsNotADuplicate(code, 0);

            // Add the airport and save changes
            var airport = new Airport
            {
                Code = code,
                Name = name.CleanString(),
                CountryId = countryId
            };

            await _factory.Context.Airports.AddAsync(airport);
            await _factory.Context.SaveChangesAsync();

            // Load the related country
            await _factory.Context.Entry(airport).Reference(m => m.Country).LoadAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Added airport {airport}");

            return airport;
        }

        /// <summary>
        /// Update an airport
        /// </summary>
        /// <param name="id"></param>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public async Task<Airport> UpdateAsync(long id, string code, string name, long countryId)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Udating airport: ID = {id}, IATA Code = {code}, Name = {name}, Country ID = {countryId}");

            // Retrieve the airport
            var airport = await _factory.Context.Airports.FirstOrDefaultAsync(x => x.Id == id);
            if (airport == null)
            {
                var message = $"Airport with ID {id} not found";
                throw new AirportNotFoundException(message);
            }

            // Check the airport doesn't already exist
            code = code.CleanString().ToUpper();            
            await CheckAirportIsNotADuplicate(code, 0);

            // Update the airport properties and save changes
            airport.Name = name;
            airport.Name = name.CleanString();
            airport.CountryId = countryId;
            await _factory.Context.SaveChangesAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Updated airport {airport}");

            return airport;
        }

        /// <summary>
        /// Delete the airport with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="AirportNotFoundException"></exception>
        public async Task DeleteAsync(long id)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Deleting airport: ID = {id}");

            // Check the airport exists
            var airport = await _factory.Context.Airports.FirstOrDefaultAsync(x => x.Id == id);
            if (airport == null)
            {
                var message = $"Airport with ID {id} not found";
                throw new AirportNotFoundException(message);
            }

            // Check there are no sightings for this airport
            var sightings = await _factory.Sightings.ListAsync(
                x => (x.Flight.Embarkation == airport.Code) || (x.Flight.Destination == airport.Code),
                1, 1).ToListAsync();
            if (sightings.Any())
            {
                var message = $"Airport with Id {id} has flights associated with it and cannot be deleted";
                throw new AirportInUseException(message);
            }

            // Remove the airport
            _factory.Context.Remove(airport);
            await _factory.Context.SaveChangesAsync();
        }

        /// <summary>
        /// Raise an exception if an attempt is made to add/update an airport with a duplicate
        /// IATA Code
        /// </summary>
        /// <param code="name"></param>
        /// <param name="id"></param>
        /// <exception cref="AirportExistsException"></exception>
        private async Task CheckAirportIsNotADuplicate(string code, long id)
        {
            var airport = await _factory.Context.Airports.FirstOrDefaultAsync(x => x.Code == code);
            if ((airport != null) && (airport.Id != id))
            {
                var message = $"Airport {code} already exists";
                throw new AirportExistsException(message);
            }
        }
    }
}
