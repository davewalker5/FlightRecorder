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

namespace FlightRecorder.BusinessLogic.Logic
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
        public Model Get(Expression<Func<Model, bool>> predicate)
            => List(predicate, 1, 1).FirstOrDefault();

        /// <summary>
        /// Get the first model matching the specified criteria along with the associated manufacturer
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<Model> GetAsync(Expression<Func<Model, bool>> predicate)
        {
            List<Model> models = await _factory.Context.Models
                                                       .Where(predicate)
                                                       .ToListAsync();
            return models.FirstOrDefault();
        }

        /// <summary>
        /// Get the models matching the specified criteria along with the associated manufacturers
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<Model> List(Expression<Func<Model, bool>> predicate, int pageNumber, int pageSize)
        {
            IEnumerable<Model> models;

            if (predicate == null)
            {
                models = _factory.Context.Models
                                         .Include(m => m.Manufacturer)
                                         .Skip((pageNumber - 1) * pageSize)
                                         .Take(pageSize);
            }
            else
            {
                models = _factory.Context.Models
                                         .Include(m => m.Manufacturer)
                                         .Skip((pageNumber - 1) * pageSize)
                                         .Take(pageSize)
                                         .Where(predicate);
            }

            return models;
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
        /// Get the models for a named manufacturer
        /// </summary>
        /// <param name="manufacturerName"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<Model> ListByManufacturer(string manufacturerName, int pageNumber, int pageSize)
        {
            manufacturerName = manufacturerName.CleanString();
            IEnumerable<Model> models = _factory.Context.Models
                                                        .Include(m => m.Manufacturer)
                                                        .Where(m => m.Manufacturer.Name == manufacturerName)
                                                        .Skip((pageNumber - 1) * pageSize)
                                                        .Take(pageSize);
            return models;
        }

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
        public Model Add(string name, string manufacturerName)
        {
            name = name.CleanString();
            Model model = Get(a => a.Name == name);

            if (model == null)
            {
                Manufacturer manufacturer = _factory.Manufacturers.Add(manufacturerName);
                model = new Model { Name = name, ManufacturerId = manufacturer.Id };
                _factory.Context.Models.Add(model);
                _factory.Context.SaveChanges();
                _factory.Context.Entry(model).Reference(m => m.Manufacturer).Load();
            }

            return model;
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
