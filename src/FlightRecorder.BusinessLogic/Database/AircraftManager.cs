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
                                           .OrderBy(a => a.Registration)
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
                                           .OrderBy(a => a.Registration)
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
        public async Task<Aircraft> AddAsync(string registration, string serialNumber, long? yearOfManufacture, long? modelId)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Adding aircraft: Registration = {registration}, Serial Number = {serialNumber}, Manufactured = {yearOfManufacture}, Model ID = {modelId}");

            registration = registration.CleanString().ToUpper();
            await CheckAircraftIsNotADuplicate(registration, 0);

            // Similarly, the serial number should be cleaned and then treated as null when assigning
            // to the new aircraft, below, if the result is empty
            var cleanSerialNumber = serialNumber.CleanString().ToUpper();
            var haveSerialNumber = !string.IsNullOrEmpty(cleanSerialNumber);

            // Finally, year of manufacture should only be stored if we have a model or serial number
            long? manufactured = haveSerialNumber && (modelId != null) ? yearOfManufacture : null;

            var aircraft = new Aircraft
            {
                Registration = registration,
                SerialNumber = haveSerialNumber ? cleanSerialNumber : null,
                Manufactured = manufactured,
                ModelId = modelId
            };

            // Save changes and reload the associated model and manufacturer
            await _factory.Context.AddAsync(aircraft);
            await _factory.Context.SaveChangesAsync();
            await _factory.Context.Entry(aircraft).Reference(m => m.Model).LoadAsync();
            if (aircraft.Model != null)
            {
                await _factory.Context.Entry(aircraft.Model).Reference(m => m.Manufacturer).LoadAsync();
            }

            _factory.Logger.LogMessage(Severity.Debug, $"Added aircraft {aircraft}");

            return aircraft;
        }

        /// <summary>
        /// Update an aircraft
        /// </summary>
        /// <param name="id"></param>
        /// <param name="registration"></param>
        /// <param name="serialNumber"></param>
        /// <param name="yearOfManufacture"></param>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public async Task<Aircraft> UpdateAsync(long id, string registration, string serialNumber, long? yearOfManufacture, long? modelId)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Updating aircraft: ID = {id}, Registration = {registration}, Serial Number = {serialNumber}, Manufactured = {yearOfManufacture}, Model ID = {modelId}");

            // Retrieve the aircraft
            var aircraft = await _factory.Context.Aircraft.FirstOrDefaultAsync(x => x.Id == id);
            if (aircraft == null)
            {
                var message = $"Aircraft with ID {id} not found";
                throw new AircraftNotFoundException(message);
            }

            // Check we're not going to create a duplicate
            registration = registration.CleanString().ToUpper();
            await CheckAircraftIsNotADuplicate(registration, id);

            // The serial number should be cleaned and then treated as null when assigning
            // to the updated aircraft, below, if the result is empty
            var cleanSerialNumber = serialNumber.CleanString().ToUpper();
            var haveSerialNumber = !string.IsNullOrEmpty(cleanSerialNumber);

            // Update the aircraft properties
            aircraft.Registration = registration;
            aircraft.SerialNumber = haveSerialNumber ? cleanSerialNumber : null;
            aircraft.Manufactured = yearOfManufacture;
            aircraft.ModelId = modelId;

            // Save changes and reload the associated model and manufacturer
            await _factory.Context.SaveChangesAsync();
            await _factory.Context.Entry(aircraft).Reference(m => m.Model).LoadAsync();
            if (aircraft.Model != null)
            {
                await _factory.Context.Entry(aircraft.Model).Reference(m => m.Manufacturer).LoadAsync();
            }

            _factory.Logger.LogMessage(Severity.Debug, $"Updated aircraft {aircraft}");

            return aircraft;
        }

        /// <summary>
        /// Delete the aircraft with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="AircraftNotFoundException"></exception>
        public async Task DeleteAsync(long id)
        {
            // Check the aircraft exists
            var aircraft = await _factory.Context.Aircraft.FirstOrDefaultAsync(x => x.Id == id);
            if (aircraft == null)
            {
                var message = $"Aircraft with ID {id} not found";
                throw new AircraftNotFoundException(message);
            }

            // Check there are no sightings for this aircraft
            var sighting = _factory.Context.Sightings.FirstOrDefault(x => x.AircraftId == id);
            if (sighting != null)
            {
                var message = $"Aircraft with Id {id} has sightings associated with it and cannot be deleted";
                throw new AircraftInUseException(message);
            }

            // Remove the aircraft
            _factory.Context.Remove(aircraft);
            await _factory.Context.SaveChangesAsync();
        }

        /// <summary>
        /// Raise an exception if an attempt is made to add/update an aircraft with a duplicate
        /// registration
        /// </summary>
        /// <param name="registration"></param>
        /// <param name="id"></param>
        /// <exception cref="BeverageExistsException"></exception>
        private async Task CheckAircraftIsNotADuplicate(string registration, long id)
        {
            var aircraft = await _factory.Context.Aircraft.FirstOrDefaultAsync(x => x.Registration == registration);
            if ((aircraft != null) && (aircraft.Id != id))
            {
                var message = $"Aircraft {registration} already exists";
                throw new AircraftExistsException(message);
            }
        }
    }
}
