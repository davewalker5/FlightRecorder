using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        {
            return List(predicate).FirstOrDefault();
        }

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> List(Expression<Func<T, bool>> predicate = null)
        {
            DbSet<T> entities = _context.Set<T>() as DbSet<T>;
            IEnumerable<T> results = (predicate == null) ? entities : entities.Where(predicate).AsQueryable();
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
    }
}
