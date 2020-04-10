using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FlightRecorder.BusinessLogic.Extensions;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlightRecorder.BusinessLogic.Logic
{
    internal class FlightManager : IFlightManager
    {
        private FlightRecorderFactory _factory;

        internal FlightManager(FlightRecorderFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Get the first flight matching the specified criteria along with the associated airline
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Flight Get(Expression<Func<Flight, bool>> predicate = null)
            => List(predicate).FirstOrDefault();

        /// <summary>
        /// Get the flights matching the specified criteria along with the associated airlines
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<Flight> List(Expression<Func<Flight, bool>> predicate = null)
        {
            IEnumerable<Flight> flights;

            if (predicate == null)
            {
                flights = _factory.Context.Flights
                                         .Include(m => m.Airline);
            }
            else
            {
                flights = _factory.Context.Flights
                                         .Include(m => m.Airline)
                                         .Where(predicate);
            }

            return flights;
        }

        /// <summary>
        /// Get the flights for a named airline
        /// </summary>
        /// <param name="airlineName"></param>
        /// <returns></returns>
        public IEnumerable<Flight> ListByAirline(string airlineName)
        {
            IEnumerable<Flight> matches = null;

            airlineName = airlineName.CleanString();
            Airline airline = _factory.Airlines.Get(m => m.Name == airlineName);
            if (airline != null)
            {
                matches = List(m => m.AirlineId == airline.Id);
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
        public Flight Add(string number, string embarkation, string destination, string airlineName)
        {
            number = number.CleanString().ToUpper();
            embarkation = embarkation.CleanString().ToUpper();
            destination = destination.CleanString().ToUpper();
            Flight flight = Get(a =>    (a.Number == number) &&
                                        (a.Embarkation == embarkation) &&
                                        (a.Destination == destination));

            if (flight == null)
            {
                Airline airline = _factory.Airlines.Add(airlineName);

                flight = new Flight
                {
                    Number = number,
                    Embarkation = embarkation,
                    Destination = destination,
                    AirlineId = airline.Id
                };

                _factory.Context.Flights.Add(flight);
                _factory.Context.SaveChanges();
                _factory.Context.Entry(flight).Reference(m => m.Airline).Load();
            }

            return flight;
        }
    }
}
