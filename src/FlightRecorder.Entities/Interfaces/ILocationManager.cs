using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Entities.Interfaces
{
    public interface ILocationManager
    {
        Location Get(Expression<Func<Location, bool>> predicate = null);
        IEnumerable<Location> List(Expression<Func<Location, bool>> predicate = null);
        Location Add(string name);
    }
}