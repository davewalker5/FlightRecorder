using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FlightRecorder.BusinessLogic.Base;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Data;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Interfaces;

namespace FlightRecorder.BusinessLogic.Logic
{
    internal class AircraftManager : ManagerBase<Aircraft>, IAircraftManager
    {
        private FlightRecorderFactory _factory;

        internal AircraftManager(FlightRecorderDbContext context, FlightRecorderFactory factory) : base(context)
        {
            _factory = factory;
        }

        /// <summary>
        /// Add an aircraft
        /// </summary>
        /// <param name="registration"></param>
        /// <param name="serialNumber"></param>
        /// <param name="yearOfManufacture"></param>
        /// <param name="model"></param>
        /// <param name="manufacturer"></param>
        /// <returns></returns>
        public Aircraft Add(string registration, string serialNumber, long yearOfManufacture, string modelName, string manufacturerName)
        {
            Aircraft aircraft = Get(a => a.Registration == registration);

            if (aircraft == null)
            {
                Model model = _factory.Models.Add(modelName, manufacturerName);

                aircraft = Add(new Aircraft
                {
                    Registration = registration,
                    SerialNumber = serialNumber,
                    Manufactured = yearOfManufacture,
                    ModelId = model.Id
                });

                _context.Entry(aircraft).Reference(m => m.Model).Load();
                _context.Entry(aircraft.Model).Reference(m => m.Manufacturer).Load();
            }

            return aircraft;
        }

        /// <summary>
        /// Get the first aircraft matching the specified criteria along with the associated model
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public override Aircraft Get(Expression<Func<Aircraft, bool>> predicate = null)
        {
            Aircraft aircraft = base.Get(predicate);

            if (aircraft != null)
            {
                _context.Entry(aircraft).Reference(m => m.Model).Load();
                _context.Entry(aircraft.Model).Reference(m => m.Manufacturer).Load();
            }

            return aircraft;
        }

        /// <summary>
        /// Get the aircraft matching the specified criteria along with the associated models
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public override IEnumerable<Aircraft> List(Expression<Func<Aircraft, bool>> predicate = null)
        {
            IEnumerable<Aircraft> aircraft = base.List(predicate);

            foreach (Aircraft aeroplane in aircraft)
            {
                _context.Entry(aeroplane).Reference(m => m.Model).Load();
                _context.Entry(aeroplane.Model).Reference(m => m.Manufacturer).Load();
            }

            return aircraft;
        }

        /// <summary>
        /// Get the aircraft of a specified model
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public IEnumerable<Aircraft> ListByModel(string modelName)
        {
            IEnumerable<Aircraft> matches = null;

            Model model = _factory.Models.Get(m => m.Name == modelName);
            if (model != null)
            {
                matches = List(m => m.ModelId == model.Id);
            }

            return matches;
        }

        /// <summary>
        /// Get the aircraft manufactured by a given manufacturer
        /// </summary>
        /// <param name="manufacturerName"></param>
        /// <returns></returns>
        public IEnumerable<Aircraft> ListByManufacturer(string manufacturerName)
        {
            IEnumerable<Aircraft> matches = null;

            Manufacturer manufacturer = _factory.Manufacturers.Get(m => m.Name == manufacturerName);
            if (manufacturer != null)
            {
                IEnumerable<long> modelIds = _factory.Models.List(m => m.ManufacturerId == manufacturer.Id).Select(m => m.Id);
                if (modelIds.Any())
                {
                    matches = List(a => modelIds.Contains(a.ModelId));
                }
            }

            return matches;
        }
    }
}
