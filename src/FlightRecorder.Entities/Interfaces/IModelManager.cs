using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IModelManager
    {
        Model Add(string name, string manufacturerName);
        Model Get(Expression<Func<Model, bool>> predicate = null);
        IEnumerable<Model> List(Expression<Func<Model, bool>> predicate = null);
        IEnumerable<Model> ListByManufacturer(string manufacturerName);
    }
}