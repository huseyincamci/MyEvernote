using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MyEvernote.Core.DataAccess
{
    public interface IDataAccess<T>
    {
        List<T> List();

        IQueryable<T> ListQueryable();

        IQueryable<T> List(Expression<Func<T, bool>> predicate);

        int Insert(T entity);

        int Update(T entity);

        int Delete(T entity);

        int Save();

        T Find(Expression<Func<T, bool>> predicate);
    }
}
