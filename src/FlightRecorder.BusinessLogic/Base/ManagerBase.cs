using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FlightRecorder.Data;
using FlightRecorder.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlightRecorder.BusinessLogic.Base
{
    internal abstract class ManagerBase<T> : IManagerBase<T> where T : class
    {
        protected FlightRecorderDbContext _context;

        internal ManagerBase(FlightRecorderDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Return the first entity matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual T Get(Expression<Func<T, bool>> predicate = null)
            => List(predicate).FirstOrDefault();

        /// <summary>
        /// Return the first entity matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<T> GetAsync(Expression<Func<T, bool>> predicate = null)
            => await ListAsync(predicate).FirstOrDefaultAsync();

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> List(Expression<Func<T, bool>> predicate = null)
        {
            DbSet<T> entities = _context.Set<T>() as DbSet<T>;
            IQueryable<T> results = (predicate == null) ? entities : entities.Where(predicate);
            return results;
        }

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual IAsyncEnumerable<T> ListAsync(Expression<Func<T, bool>> predicate = null)
        {
            DbSet<T> entities = _context.Set<T>() as DbSet<T>;
            IAsyncEnumerable<T> results = (predicate == null) ? entities.AsAsyncEnumerable()
                : entities.Where(predicate).AsAsyncEnumerable(); ;
            return results;
        }

        /// <summary>
        /// Add a new entity and save changes
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public T Add(T entity)
        {
            DbSet<T> entities = _context.Set<T>() as DbSet<T>;
            entities.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        /// <summary>
        /// Add a new entity and save changes
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<T> AddAsync(T entity)
        {
            DbSet<T> entities = _context.Set<T>() as DbSet<T>;
            entities.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
