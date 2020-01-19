using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IFlightManager
    {
        Flight Add(string number, string embarkation, string destination, string airlineName);
        Flight Get(Expression<Func<Flight, bool>> predicate = null);
        IEnumerable<Flight> List(Expression<Func<Flight, bool>> predicate = null);
        IEnumerable<Flight> ListByAirline(string airlineName);
    }
}