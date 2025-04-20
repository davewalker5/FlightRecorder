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
    internal class ModelManager : IModelManager
    {
        private readonly FlightRecorderFactory _factory;

        internal ModelManager(FlightRecorderFactory factory)
        {
            _factory = factory;
        }

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
                                         .Skip((pageNumber - 1) * pageSize)
                                         .Take(pageSize)
                                         .AsAsyncEnumerable();
            }
            else
            {
                models = _factory.Context.Models
                                         .Include(m => m.Manufacturer)
                                         .Where(predicate)
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
        /// <param name="manufacturerName"></param>
        /// <returns></returns>
        public async Task<Model> AddAsync(string name, string manufacturerName)
        {
            name = name.CleanString();
            Model model = await GetAsync(a => a.Name == name);

            if (model == null)
            {
                Manufacturer manufacturer = await _factory.Manufacturers.AddAsync(manufacturerName);
                model = new Model { Name = name, ManufacturerId = manufacturer.Id };
                await _factory.Context.Models.AddAsync(model);
                await _factory.Context.SaveChangesAsync();
                await _factory.Context.Entry(model).Reference(m => m.Manufacturer).LoadAsync();
            }

            return model;
        }
    }
}
