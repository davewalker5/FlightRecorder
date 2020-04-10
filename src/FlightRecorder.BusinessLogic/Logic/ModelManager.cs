using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FlightRecorder.BusinessLogic.Base;
using FlightRecorder.BusinessLogic.Extensions;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Data;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlightRecorder.BusinessLogic.Logic
{
    internal class ModelManager : IModelManager
    {
        private FlightRecorderFactory _factory;

        internal ModelManager(FlightRecorderFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Get the first model matching the specified criteria along with the associated manufacturer
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Model Get(Expression<Func<Model, bool>> predicate = null)
            => List(predicate).FirstOrDefault();

        /// <summary>
        /// Get the models matching the specified criteria along with the associated manufacturers
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<Model> List(Expression<Func<Model, bool>> predicate = null)
        {
            IEnumerable<Model> models;

            if (predicate == null)
            {
                models = _factory.Context.Models
                                         .Include(m => m.Manufacturer);
            }
            else
            {
                models = _factory.Context.Models
                                         .Include(m => m.Manufacturer)
                                         .Where(predicate);
            }

            return models;
        }

        /// <summary>
        /// Get the models for a named manufacturer
        /// </summary>
        /// <param name="manufacturerName"></param>
        /// <returns></returns>
        public IEnumerable<Model> ListByManufacturer(string manufacturerName)
        {
            manufacturerName = manufacturerName.CleanString();
            IEnumerable<Model> models = _factory.Context.Models
                                                        .Include(m => m.Manufacturer)
                                                        .Where(m => m.Manufacturer.Name == manufacturerName);
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
    }
}
