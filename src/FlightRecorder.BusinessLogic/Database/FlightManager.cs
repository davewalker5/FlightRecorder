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
                                         .Skip((pageNumber - 1) * pageSize)
                                         .Take(pageSize)
                                         .AsAsyncEnumerable();
            }
            else
            {
                flights = _factory.Context.Flights
                                         .Include(m => m.Airline)
                                         .Where(predicate)
                                         .Skip((pageNumber - 1) * pageSize)
                                         .Take(pageSize)
                                         .AsAsyncEnumerable();
            }

            return flights;
        }

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
        /// <param name="airlineName"></param>
        /// <returns></returns>
        public async Task<Flight> AddAsync(string number, string embarkation, string destination, string airlineName)
        {
            number = number.CleanString().ToUpper();
            embarkation = embarkation.CleanString().ToUpper();
            destination = destination.CleanString().ToUpper();
            Flight flight = await GetAsync(a => (a.Number == number) &&
                                                (a.Embarkation == embarkation) &&
                                                (a.Destination == destination));

            if (flight == null)
            {
                Airline airline = await _factory.Airlines.AddAsync(airlineName);

                flight = new Flight
                {
                    Number = number,
                    Embarkation = embarkation,
                    Destination = destination,
                    AirlineId = airline.Id
                };

                await _factory.Context.Flights.AddAsync(flight);
                await _factory.Context.SaveChangesAsync();
                await _factory.Context.Entry(flight).Reference(m => m.Airline).LoadAsync();
            }

            return flight;
        }
    }
}
