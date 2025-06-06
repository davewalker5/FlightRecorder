﻿using System;
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
    internal class AircraftManager : IAircraftManager
    {
        private const int AllModelsPageSize = 1000000;

        private readonly FlightRecorderFactory _factory;

        internal AircraftManager(FlightRecorderFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Get the first aircraft matching the specified criteria along with the associated model
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<Aircraft> GetAsync(Expression<Func<Aircraft, bool>> predicate)
        {
            List<Aircraft> aircraft = await ListAsync(predicate, 1, 1).ToListAsync();
            return aircraft.FirstOrDefault();
        }

        /// <summary>
        /// Get the aircraft matching the specified criteria along with the associated models
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IAsyncEnumerable<Aircraft> ListAsync(Expression<Func<Aircraft, bool>> predicate, int pageNumber, int pageSize)
        {
            IAsyncEnumerable<Aircraft> aircraft;

            if (predicate == null)
            {
                aircraft = _factory.Context.Aircraft
                                           .Include(a => a.Model)
                                           .ThenInclude(m => m.Manufacturer)
                                           .Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .AsAsyncEnumerable();
            }
            else
            {
                aircraft = _factory.Context.Aircraft
                                           .Include(a => a.Model)
                                           .ThenInclude(m => m.Manufacturer)
                                           .Where(predicate)
                                           .Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .AsAsyncEnumerable();
            }

            return aircraft;
        }

        /// <summary>
        /// Return the number of aircraft in the database
        /// </summary>
        /// <returns></returns>
        public async Task<int> CountAsync()
            => await _factory.Context.Aircraft.CountAsync();

        /// <summary>
        /// Get the aircraft of a specified model
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IAsyncEnumerable<Aircraft>> ListByModelAsync(string modelName, int pageNumber, int pageSize)
        {
            IAsyncEnumerable<Aircraft> matches = null;

            modelName = modelName.CleanString();
            Model model = await _factory.Models.GetAsync(m => m.Name == modelName);
            if (model != null)
            {
                matches = ListAsync(m => m.ModelId == model.Id, pageNumber, pageSize);
            }

            return matches;
        }

        /// <summary>
        /// Get the aircraft manufactured by a given manufacturer
        /// </summary>
        /// <param name="manufacturerName"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IAsyncEnumerable<Aircraft>> ListByManufacturerAsync(string manufacturerName, int pageNumber, int pageSize)
        {
            IAsyncEnumerable<Aircraft> matches = null;

            manufacturerName = manufacturerName.CleanString();
            Manufacturer manufacturer = await _factory.Manufacturers
                                                      .GetAsync(m => m.Name == manufacturerName);
            if (manufacturer != null)
            {
                // Model retrieval uses an arbitrarily large page size to retrieve all models
                List<long> modelIds = await _factory.Models
                                                    .ListAsync(m => m.ManufacturerId == manufacturer.Id, 1, AllModelsPageSize)
                                                    .Select(m => m.Id)
                                                    .ToListAsync();
                if (modelIds.Any())
                {
                    matches = ListAsync(a => modelIds.Contains(a.ModelId ?? -1), pageNumber, pageSize);
                }
            }

            return matches;
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
        public async Task<Aircraft> AddAsync(string registration, string serialNumber, long? yearOfManufacture, string modelName, string manufacturerName)
        {
            registration = registration.CleanString().ToUpper();
            Aircraft aircraft = await GetAsync(a => a.Registration == registration);

            if (aircraft == null)
            {
                // If partial aircraft details have been supplied, the manufacturer and/or model may not
                // be specified
                long? modelId = null;
                if (!string.IsNullOrEmpty(manufacturerName) && !string.IsNullOrEmpty(modelName))
                {
                    Model model = await _factory.Models.AddAsync(modelName, manufacturerName);
                    modelId = model.Id;
                }

                // Similarly, the serial number should be cleaned and then treated as null when assigning
                // to the new aircraft, below, if the result is empty
                var cleanSerialNumber = serialNumber.CleanString().ToUpper();
                var haveSerialNumber = !string.IsNullOrEmpty(cleanSerialNumber);

                // Finally, year of manufacture should only be stored if we have a model or serial number
                long? manufactured = haveSerialNumber && (modelId != null) ? yearOfManufacture : null;

                aircraft = new Aircraft
                {
                    Registration = registration,
                    SerialNumber = haveSerialNumber ? cleanSerialNumber : null,
                    Manufactured = manufactured,
                    ModelId = modelId
                };

                await _factory.Context.AddAsync(aircraft);
                await _factory.Context.SaveChangesAsync();
                await _factory.Context.Entry(aircraft).Reference(m => m.Model).LoadAsync();
                if (aircraft.Model != null)
                {
                    await _factory.Context.Entry(aircraft.Model).Reference(m => m.Manufacturer).LoadAsync();
                }
            }

            return aircraft;
        }
    }
}
