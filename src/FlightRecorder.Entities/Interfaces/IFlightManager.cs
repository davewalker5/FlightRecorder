﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IFlightManager
    {
        Task<Flight> AddAsync(string number, string embarkation, string destination, string airlineName);
        Task<Flight> GetAsync(Expression<Func<Flight, bool>> predicate);
        IAsyncEnumerable<Flight> ListAsync(Expression<Func<Flight, bool>> predicate, int pageNumber, int pageSize);
        Task<int> CountAsync();
        Task<IAsyncEnumerable<Flight>> ListByAirlineAsync(string airlineName, int pageNumber, int pageSize);
    }
}