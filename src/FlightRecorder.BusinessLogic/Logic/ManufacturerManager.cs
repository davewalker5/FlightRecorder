﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FlightRecorder.BusinessLogic.Extensions;
using FlightRecorder.Data;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Interfaces;

namespace FlightRecorder.BusinessLogic.Logic
{
    internal class ManufacturerManager : IManufacturerManager
    {
        private FlightRecorderDbContext _context;

        internal ManufacturerManager(FlightRecorderDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Return the first entity matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Manufacturer Get(Expression<Func<Manufacturer, bool>> predicate = null)
            => List(predicate).FirstOrDefault();

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual IEnumerable<Manufacturer> List(Expression<Func<Manufacturer, bool>> predicate = null)
        {
            IEnumerable<Manufacturer> results;
            if (predicate == null)
            {
                results = _context.Manufacturers;
            }
            else
            {
                results = _context.Manufacturers.Where(predicate);
            }

            return results;
        }

        /// <summary>
        /// Add a named manufacturer, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Manufacturer Add(string name)
        {
            name = name.CleanString();
            Manufacturer manufacturer = Get(a => a.Name == name);

            if (manufacturer == null)
            {
                manufacturer = new Manufacturer { Name = name };
                _context.Manufacturers.Add(manufacturer);
                _context.SaveChanges();
            }

            return manufacturer;
        }
    }
}
