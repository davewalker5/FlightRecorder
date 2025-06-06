﻿using FlightRecorder.Entities.Db;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FlightRecorder.Entities.Interfaces
{
    public interface ICountryManager
    {
        Task<Country> AddAsync(string name);
        Task<Country> GetAsync(Expression<Func<Country, bool>> predicate);
        IAsyncEnumerable<Country> ListAsync(Expression<Func<Country, bool>> predicate, int pageNumber, int pageSize);
    }
}