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
    internal class LocationManager : ILocationManager
    {
        private FlightRecorderDbContext _context;

        internal LocationManager(FlightRecorderDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Return the first entity matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Location Get(Expression<Func<Location, bool>> predicate = null)
            => List(predicate).FirstOrDefault();

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual IEnumerable<Location> List(Expression<Func<Location, bool>> predicate = null)
        {
            IEnumerable<Location> results;
            if (predicate == null)
            {
                results = _context.Locations;
            }
            else
            {
                results = _context.Locations.Where(predicate);
            }

            return results;
        }

        /// <summary>
        /// Add a named location, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Location Add(string name)
        {
            name = name.CleanString();
            Location location = Get(a => a.Name == name);

            if (location == null)
            {
                location = new Location { Name = name };
                _context.Locations.Add(location);
                _context.SaveChanges();
            }

            return location;
        }
    }
}
