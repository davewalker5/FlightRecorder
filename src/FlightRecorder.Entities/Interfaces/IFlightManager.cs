using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IFlightManager
    {
        Flight Add(string number, string embarkation, string destination, string airlineName);
        Task<Flight> AddAsync(string number, string embarkation, string destination, string airlineName);
        Flight Get(Expression<Func<Flight, bool>> predicate = null);
        Task<Flight> GetAsync(Expression<Func<Flight, bool>> predicate);
        IEnumerable<Flight> List(Expression<Func<Flight, bool>> predicate, int pageNumber, int pageSize);
        IAsyncEnumerable<Flight> ListAsync(Expression<Func<Flight, bool>> predicate, int pageNumber, int pageSize);
        IEnumerable<Flight> ListByAirline(string airlineName, int pageNumber, int pageSize);
        Task<IAsyncEnumerable<Flight>> ListByAirlineAsync(string airlineName, int pageNumber, int pageSize);
    }
}