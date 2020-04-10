using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IAirlineManager
    {
        Airline Get(Expression<Func<Airline, bool>> predicate = null);
        IEnumerable<Airline> List(Expression<Func<Airline, bool>> predicate = null);
        Airline Add(string name);
    }
}