using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IAircraftManager
    {
        Aircraft Add(string registration, string serialNumber, long yearOfManufacture, string modelName, string manufacturerName);
        Aircraft Get(Expression<Func<Aircraft, bool>> predicate = null);
        IEnumerable<Aircraft> List(Expression<Func<Aircraft, bool>> predicate = null);
        IEnumerable<Aircraft> ListByModel(string modelName);
        IEnumerable<Aircraft> ListByManufacturer(string manufacturerName);
    }
}