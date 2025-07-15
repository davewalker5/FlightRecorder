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
    internal class FlightManager : IFlightManager
    {
        private readonly FlightRecorderFactory _factory;

        internal FlightManager(FlightRecorderFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Get the first flight matching the specified criteria along with the associated airline
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<Flight> GetAsync(Expression<Func<Flight, bool>> predicate)
        {
            List<Flight> flights = await ListAsync(predicate, 1, 1).ToListAsync();
            return flights.FirstOrDefault();
        }

        /// <summary>
        /// Get the flights matching the specified criteria along with the associated airlines
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IAsyncEnumerable<Flight> ListAsync(Expression<Func<Flight, bool>> predicate, int pageNumber, int pageSize)
        {
            IAsyncEnumerable<Flight> flights;

            if (predicate == null)
            {
                flights = _factory.Context.Flights
                                         .Include(m => m.Airline)
                                         .OrderBy(f => f.Number)
                                         .Skip((pageNumber - 1) * pageSize)
                                         .Take(pageSize)
                                         .AsAsyncEnumerable();
            }
            else
            {
                flights = _factory.Context.Flights
                                         .Include(m => m.Airline)
                                         .OrderBy(f => f.Number)
                                         .Where(predicate)
                                         .Skip((pageNumber - 1) * pageSize)
                                         .Take(pageSize)
                                         .AsAsyncEnumerable();
            }

            return flights;
        }

        /// <summary>
        /// Return the number of flights in the database
        /// </summary>
        /// <returns></returns>
        public async Task<int> CountAsync()
            => await _factory.Context.Flights.CountAsync();

        /// <summary>
        /// Get the flights for a named airline
        /// </summary>
        /// <param name="airlineName"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IAsyncEnumerable<Flight>> ListByAirlineAsync(string airlineName, int pageNumber, int pageSize)
        {
            IAsyncEnumerable<Flight> matches = null;

            airlineName = airlineName.CleanString();
            Airline airline = await _factory.Airlines
                                            .GetAsync(m => m.Name == airlineName);
            if (airline != null)
            {
                matches = ListAsync(m => m.AirlineId == airline.Id, pageNumber, pageSize);
            }

            return matches;
        }

        /// <summary>
        /// Add a new flight
        /// </summary>
        /// <param name="number"></param>
        /// <param name="embarkation"></param>
        /// <param name="destination"></param>
        /// <param name="airlineId"></param>
        /// <returns></returns>
        public async Task<Flight> AddAsync(string number, string embarkation, string destination, long airlineId)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Adding flight: Number = {number}, Route = {embarkation}-{destination}, Airline ID = {airlineId}");

            // Check the flight doesn't already exist
            number = number.CleanString().ToUpper();
            embarkation = embarkation.CleanString().ToUpper();
            destination = destination.CleanString().ToUpper();
            await CheckFlightIsNotADuplicate(number, embarkation, destination, airlineId, 0);

            // Check the airline and the airports exist
            await _factory.Airlines.CheckAirlineExists(airlineId);
            await _factory.Airports.CheckAirportExists(embarkation);
            await _factory.Airports.CheckAirportExists(destination);

            // Add the flight and save changes
            var flight = new Flight
            {
                Number = number,
                Embarkation = embarkation,
                Destination = destination,
                AirlineId = airlineId
            };

            await _factory.Context.Flights.AddAsync(flight);
            await _factory.Context.SaveChangesAsync();

            // Load the associated airline
            await _factory.Context.Entry(flight).Reference(m => m.Airline).LoadAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Added flight {flight}");

            return flight;
        }

        /// <summary>
        /// Add a named manufacturer, if it doesn't already exist
        /// </summary>
        /// <param name="number"></param>
        /// <param name="embarkation"></param>
        /// <param name="destination"></param>
        /// <param name="airlineId"></param>
        /// <returns></returns>
        public async Task<Flight> AddIfNotExistsAsync(string number, string embarkation, string destination, long airlineId)
        {
            number = number.CleanString().ToUpper();
            embarkation = embarkation.CleanString().ToUpper();
            destination = destination.CleanString().ToUpper();

            var flight = await GetAsync(a => (a.Number == number) &&
                                                (a.Embarkation == embarkation) &&
                                                (a.Destination == destination) &&
                                                (a.AirlineId == airlineId));
            if (flight == null)
            {
                flight = await AddAsync(number, embarkation, destination, airlineId);
            }

            return flight;
        }

        /// <summary>
        /// Update a flight
        /// </summary>
        /// <param name="id"></param>
        /// <param name="number"></param>
        /// <param name="embarkation"></param>
        /// <param name="destination"></param>
        /// <param name="airlineId"></param>
        /// <returns></returns>
        public async Task<Flight> UpdateAsync(long id, string number, string embarkation, string destination, long airlineId)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Updating flight: ID = {id}, Number = {number}, Route = {embarkation}-{destination}, Airline ID = {airlineId}");

            // Retrieve the flight
            var flight = await _factory.Context.Flights.FirstOrDefaultAsync(x => x.Id == id);
            if (flight == null)
            {
                var message = $"Flight with ID {id} not found";
                throw new FlightNotFoundException(message);
            }

            // Check the flight doesn't already exist
            number = number.CleanString().ToUpper();
            embarkation = embarkation.CleanString().ToUpper();
            destination = destination.CleanString().ToUpper();
            await CheckFlightIsNotADuplicate(number, embarkation, destination, airlineId, id);

            // Check the airline and the airports exist
            await _factory.Airlines.CheckAirlineExists(airlineId);
            await _factory.Airports.CheckAirportExists(embarkation);
            await _factory.Airports.CheckAirportExists(destination);

            // Update the flight properties and save changes
            flight.Number = number;
            flight.Embarkation = embarkation;
            flight.Destination = destination;
            flight.AirlineId = airlineId;
            await _factory.Context.SaveChangesAsync();

            // Load the associated airline
            await _factory.Context.Entry(flight).Reference(m => m.Airline).LoadAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Updated flight {flight}");

            return flight;
        }

        /// <summary>
        /// Delete the flight with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="FlightNotFoundException"></exception>
        /// <exception cref="FlightInUseException"></exception>
        public async Task DeleteAsync(long id)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Deleting flight: ID = {id}");

            // Check the flight exists
            var flight = await _factory.Context.Flights.FirstOrDefaultAsync(x => x.Id == id);
            if (flight == null)
            {
                var message = $"Flight with ID {id} not found";
                throw new FlightNotFoundException(message);
            }

            // Check there are no sightings for this flight
            var sightings = await _factory.Sightings.ListAsync(x => x.FlightId == id, 1, 1).ToListAsync();
            if (sightings.Any())
            {
                var message = $"Flight with Id {id} has sightings associated with it and cannot be deleted";
                throw new FlightInUseException(message);
            }

            // Remove the flight
            _factory.Context.Remove(flight);
            await _factory.Context.SaveChangesAsync();
        }

        /// <summary>
        /// Raise an exception if the specified flight doesn't exist
        /// </summary>
        /// <param name="flightId"></param>
        /// <exception cref="FlightNotFoundException"></exception>
        public async Task CheckFlightExists(long flightId)
        {
            var flight = await _factory.Flights.GetAsync(x => x.Id == flightId);
            if (flight == null)
            {
                var message = $"Flight with ID {flightId} not found";
                throw new FlightNotFoundException(message);
            }
        }

        /// <summary>
        /// Raise an exception if an attempt is made to add/update a duplicate flight
        /// </summary>
        /// <param name="number"></param>
        /// <param name="embarkation"></param>
        /// <param name="destination"></param>
        /// <param name="airlineId"></param>
        /// <param name="id"></param>
        /// <exception cref="FlightExistsException"></exception>
        private async Task CheckFlightIsNotADuplicate(string number, string embarkation, string destination, long airlineId, long id)
        {
            var flight = await GetAsync(a => (a.Number == number) &&
                                                (a.Embarkation == embarkation) &&
                                                (a.Destination == destination) &&
                                                (a.AirlineId == airlineId));
            if ((flight != null) && (flight.Id != id))
            {
                var message = $"Flight {number} with route {embarkation}-{destination} for airline with ID {airlineId} already exists";
                throw new FlightExistsException(message);
            }
        }
    }
}
