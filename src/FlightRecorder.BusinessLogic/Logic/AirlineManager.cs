using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FlightRecorder.BusinessLogic.Extensions;
using FlightRecorder.Data;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Interfaces;

namespace FlightRecorder.BusinessLogic.Logic
{
    internal class AirlineManager : IAirlineManager
    {
        private FlightRecorderDbContext _context;

        internal AirlineManager(FlightRecorderDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Return the first entity matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Airline Get(Expression<Func<Airline, bool>> predicate = null)
            => List(predicate).FirstOrDefault();

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual IEnumerable<Airline> List(Expression<Func<Airline, bool>> predicate = null)
        {
            IEnumerable<Airline> results;
            if (predicate == null)
            {
                results = _context.Airlines;
            }
            else
            {
                results = _context.Airlines.Where(predicate);
            }

            return results;
        }

        /// <summary>
        /// Add a named airline, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Airline Add(string name)
        {
            name = name.CleanString();
            Airline airline = Get(a => a.Name == name);

            if (airline == null)
            {
                airline = new Airline { Name = name };
                _context.Airlines.Add(airline);
                _context.SaveChanges();
            }

            return airline;
        }
    }
}
