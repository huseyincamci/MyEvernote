using MyEvernote.Common;
using MyEvernote.Core.DataAccess;
using MyEvernote.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace MyEvernote.DataAccessLayer.EntityFramework
{
    public class Repository<T> : RepositoryBase, IDataAccess<T> where T : class
    {
        private DbSet<T> _dbSet;

        public Repository()
        {
            _dbSet = _context.Set<T>();
        }

        public List<T> List()
        {
            return _dbSet.ToList();
        }

        public IQueryable<T> ListQueryable()
        {
            return _dbSet.AsQueryable<T>();
        }

        public IQueryable<T> List(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public int Insert(T entity)
        {
            _dbSet.Add(entity);
            if (entity is MyEntityBase)
            {
                MyEntityBase o = entity as MyEntityBase;
                DateTime now = DateTime.Now;
                o.CreatedOn = now;
                o.ModifiedOn = now;
                o.ModifiedUsername = App.common.GetCurrentUsername();
            }
            return Save();
        }

        public int Update(T entity)
        {
            if (entity is MyEntityBase)
            {
                MyEntityBase o = entity as MyEntityBase;
                o.ModifiedOn = DateTime.Now;
                o.ModifiedUsername = App.common.GetCurrentUsername();
            }
            return Save();
        }

        public int Delete(T entity)
        {
            _dbSet.Remove(entity);
            return Save();
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public T Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }
    }
}
