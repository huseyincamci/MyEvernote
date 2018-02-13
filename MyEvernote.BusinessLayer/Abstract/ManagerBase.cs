using MyEvernote.Core.DataAccess;
using MyEvernote.DataAccessLayer.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MyEvernote.BusinessLayer.Abstract
{
    public abstract class ManagerBase<T> : IDataAccess<T> where T : class
    {
        private Repository<T> _repo = new Repository<T>();

        public virtual int Delete(T entity)
        {
            return _repo.Delete(entity);
        }

        public virtual T Find(Expression<Func<T, bool>> predicate)
        {
            return _repo.Find(predicate);
        }

        public virtual int Insert(T entity)
        {
            return _repo.Insert(entity);
        }

        public virtual List<T> List()
        {
            return _repo.List();
        }

        public virtual IQueryable<T> List(Expression<Func<T, bool>> predicate)
        {
            return _repo.List(predicate);
        }

        public virtual IQueryable<T> ListQueryable()
        {
            return _repo.ListQueryable();
        }

        public virtual int Save()
        {
           return _repo.Save();
        }

        public virtual int Update(T entity)
        {
            return _repo.Update(entity);
        }
    }
}
