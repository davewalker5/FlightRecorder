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
    internal class ModelManager : IModelManager
    {
        private readonly FlightRecorderFactory _factory;

        internal ModelManager(FlightRecorderFactory factory)
            => _factory = factory;

        /// <summary>
        /// Get the first model matching the specified criteria along with the associated manufacturer
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<Model> GetAsync(Expression<Func<Model, bool>> predicate)
        {
            List<Model> models = await ListAsync(predicate, 1, 1).ToListAsync();
            return models.FirstOrDefault();
        }

        /// <summary>
        /// Get the models matching the specified criteria along with the associated manufacturers
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IAsyncEnumerable<Model> ListAsync(Expression<Func<Model, bool>> predicate, int pageNumber, int pageSize)
        {
            IAsyncEnumerable<Model> models;

            if (predicate == null)
            {
                models = _factory.Context.Models
                                         .Include(m => m.Manufacturer)
                                         .OrderBy(m => m.Name)
                                         .Skip((pageNumber - 1) * pageSize)
                                         .Take(pageSize)
                                         .AsAsyncEnumerable();
            }
            else
            {
                models = _factory.Context.Models
                                         .Include(m => m.Manufacturer)
                                         .Where(predicate)
                                         .OrderBy(m => m.Name)
                                         .Skip((pageNumber - 1) * pageSize)
                                         .Take(pageSize)
                                         .AsAsyncEnumerable();
            }

            return models;
        }

        /// <summary>
        /// Return the number of aircraft models in the database
        /// </summary>
        /// <returns></returns>
        public async Task<int> CountAsync()
            => await _factory.Context.Models.CountAsync();

        /// <summary>
        /// Get the models for a named manufacturer
        /// </summary>
        /// <param name="manufacturerName"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IAsyncEnumerable<Model> ListByManufacturerAsync(string manufacturerName, int pageNumber, int pageSize)
        {
            manufacturerName = manufacturerName.CleanString();
            IAsyncEnumerable<Model> models = _factory.Context.Models
                                                             .Include(m => m.Manufacturer)
                                                             .Where(m => m.Manufacturer.Name == manufacturerName)
                                                             .Skip((pageNumber - 1) * pageSize)
                                                             .Take(pageSize)
                                                             .AsAsyncEnumerable();
            return models;
        }

        /// <summary>
        /// Add a named model associated with the specified manufacturer, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <param name="manufacturerId"></param>
        /// <returns></returns>
        public async Task<Model> AddAsync(string name, long manufacturerId)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Adding model: Name = {name}, Manufacturer ID = {manufacturerId}");

            // Check the model doesn't already exist
            name = name.CleanString().ToUpper();
            await CheckModelIsNotADuplicate(name, manufacturerId, 0);

            // Check the manufacturer exists
            await _factory.Manufacturers.CheckManufacturerExists(manufacturerId);

            // Add the model and save changes
            var model = new Model
            {
                Name = name,
                ManufacturerId = manufacturerId
            };

            await _factory.Context.Models.AddAsync(model);
            await _factory.Context.SaveChangesAsync();

            // Load the associated manufacturer
            await _factory.Context.Entry(model).Reference(m => m.Manufacturer).LoadAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Added model {model}");

            return model;
        }

        /// <summary>
        /// Add a named model, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <param name="manufacturerId"></param>
        /// <returns></returns>
        public async Task<Model> AddIfNotExistsAsync(string name, long manufacturerId)
        {
            name = name.CleanString();
            var model = await GetAsync(x => (x.Name == name) && (x.ManufacturerId == manufacturerId));
            if (model == null)
            {
                model = await AddAsync(name, manufacturerId);
            }
            return model;
        }

        /// <summary>
        /// Update a model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="manufacturerId"></param>
        /// <returns></returns>
        public async Task<Model> UpdateAsync(long id, string name, long manufacturerId)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Updating model: ID = {id}, Name = {name}, Manufacturer ID = {manufacturerId}");

            // Retrieve the model
            var model = await _factory.Context.Models.FirstOrDefaultAsync(x => x.Id == id);
            if (model == null)
            {
                var message = $"Model with ID {id} not found";
                throw new ModelNotFoundException(message);
            }

            // Check the model doesn't already exist
            name = name.CleanString().ToUpper();
            await CheckModelIsNotADuplicate(name, manufacturerId, id);

            // Check the airline exists
            await _factory.Manufacturers.CheckManufacturerExists(manufacturerId);

            // Update the model properties and save changes
            model.Name = name;
            model.ManufacturerId = manufacturerId;
            await _factory.Context.SaveChangesAsync();

            // Load the associated manufacturer
            await _factory.Context.Entry(model).Reference(m => m.Manufacturer).LoadAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Updated model {model}");

            return model;
        }

        /// <summary>
        /// Delete the model with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ModelNotFoundException"></exception>
        /// <exception cref="ModelInUseException"></exception>
        public async Task DeleteAsync(long id)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Deleting model: ID = {id}");

            // Check the model exists
            var model = await _factory.Context.Models.FirstOrDefaultAsync(x => x.Id == id);
            if (model == null)
            {
                var message = $"Model with ID {id} not found";
                throw new ModelNotFoundException(message);
            }

            // Check there are no aircraft for this model
            var aircraft = await _factory.Aircraft.ListAsync(x => x.ModelId == id, 1, 1).ToListAsync();
            if (aircraft.Any())
            {
                var message = $"Model with Id {id} has aircraft associated with it and cannot be deleted";
                throw new ModelInUseException(message);
            }

            // Remove the model
            _factory.Context.Remove(model);
            await _factory.Context.SaveChangesAsync();
        }

        /// <summary>
        /// Raise an exception if the specified model doesn't exist
        /// </summary>
        /// <param name="modelId"></param>
        /// <exception cref="ModelNotFoundException"></exception>
        public async Task CheckModelExists(long? modelId)
        {
            if (modelId != null)
            {
                var model = await _factory.Models.GetAsync(x => x.Id == modelId);
                if (model == null)
                {
                    var message = $"Model with ID {modelId} not found";
                    throw new ModelNotFoundException(message);
                }
            }
        }

        /// <summary>
        /// Raise an exception if an attempt is made to add/update a duplicate model
        /// </summary>
        /// <param name="name"></param>
        /// <param name="manufacturerId"></param>
        /// <param name="id"></param>
        /// <exception cref="ModelExistsException"></exception>
        private async Task CheckModelIsNotADuplicate(string name, long manufacturerId, long id)
        {
            var model = await GetAsync(a => (a.Name == name) && (a.ManufacturerId == manufacturerId));
            if ((model != null) && (model.Id != id))
            {
                var message = $"Model {name} for manufacturer with ID {manufacturerId} already exists";
                throw new ModelExistsException(message);
            }
        }
    }
}
