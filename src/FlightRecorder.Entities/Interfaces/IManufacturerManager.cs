using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IManufacturerManager
    {
        Manufacturer Get(Expression<Func<Manufacturer, bool>> predicate = null);
        IEnumerable<Manufacturer> List(Expression<Func<Manufacturer, bool>> predicate = null);
        Manufacturer Add(string name);
    }
}