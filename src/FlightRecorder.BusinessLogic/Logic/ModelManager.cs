using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FlightRecorder.BusinessLogic.Base;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Data;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Interfaces;

namespace FlightRecorder.BusinessLogic.Logic
{
    internal class ModelManager : ManagerBase<Model>, IModelManager
    {
        private FlightRecorderFactory _factory;

        internal ModelManager(FlightRecorderDbContext context, FlightRecorderFactory factory) : base(context)
        {
            _factory = factory;
        }

        /// <summary>
        /// Add a named model associated with the specified manufacturer, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <param name="manufacturerName"></param>
        /// <returns></returns>
        public Model Add(string name, string manufacturerName)
        {
            Model model = Get(a => a.Name == name);

            if (model == null)
            {
                Manufacturer manufacturer = _factory.Manufacturers.Add(manufacturerName);
                model = Add(new Model { Name = name, ManufacturerId = manufacturer.Id });
                _context.Entry(model).Reference(m => m.Manufacturer).Load();
            }

            return model;
        }

        /// <summary>
        /// Get the first model matching the specified criteria along with the associated manufacturer
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public override Model Get(Expression<Func<Model, bool>> predicate = null)
        {
            Model model = base.Get(predicate);

            if (model != null)
            {
                _context.Entry(model).Reference(m => m.Manufacturer).Load();
            }

            return model;
        }

        /// <summary>
        /// Get the models matching the specified criteria along with the associated manufacturers
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public override IEnumerable<Model> List(Expression<Func<Model, bool>> predicate = null)
        {
            IEnumerable<Model> models = base.List(predicate);

            foreach (Model model in models)
            {
                _context.Entry(model).Reference(m => m.Manufacturer).Load();
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
            IEnumerable<Model> matches = null;

            Manufacturer manufacturer = _factory.Manufacturers.Get(m => m.Name == manufacturerName);
            if (manufacturer != null)
            {
                matches = List(m => m.ManufacturerId == manufacturer.Id);
            }

            return matches;
        }
    }
}
