using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FlightRecorder.BusinessLogic.Base;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Data;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Interfaces;

namespace FlightRecorder.BusinessLogic.Logic
{
    internal class FlightManager : ManagerBase<Flight>, IFlightManager
    {
        private FlightRecorderFactory _factory;

        internal FlightManager(FlightRecorderDbContext context, FlightRecorderFactory factory) : base(context)
        {
            _factory = factory;
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
            Flight flight = Get(a => a.Number == number);

            if (flight == null)
            {
                Airline airline = _factory.Airlines.Add(airlineName);

                flight = Add(new Flight
                {
                    Number = number,
                    Embarkation = embarkation,
                    Destination = destination,
                    AirlineId = airline.Id
                });

                _context.Entry(flight).Reference(m => m.Airline).Load();
            }

            return flight;
        }

        /// <summary>
        /// Get the first flight matching the specified criteria along with the associated airline
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public override Flight Get(Expression<Func<Flight, bool>> predicate = null)
        {
            Flight flight = base.Get(predicate);

            if (flight != null)
            {
                _context.Entry(flight).Reference(m => m.Airline).Load();
            }

            return flight;
        }

        /// <summary>
        /// Get the flights matching the specified criteria along with the associated airlines
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public override IEnumerable<Flight> List(Expression<Func<Flight, bool>> predicate = null)
        {
            IEnumerable<Flight> flights = base.List(predicate);

            foreach (Flight flight in flights)
            {
                _context.Entry(flight).Reference(m => m.Airline).Load();
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

            Airline airline = _factory.Airlines.Get(m => m.Name == airlineName);
            if (airline != null)
            {
                matches = List(m => m.AirlineId == airline.Id);
            }

            return matches;
        }
    }
}
