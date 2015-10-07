using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AppFramework.Entities;

namespace AppFramework.DataProxy
{
    public class IncludesBuilder<TEntity> where TEntity : class, AppFramework.Entities.IDataEntity, IObjectWithChangeTracker
    {
        private List<Expression<Func<TEntity, object>>> _includes;

        public IncludesBuilder()
        {
            _includes = new List<Expression<Func<TEntity, object>>>();
        }

        public IncludesBuilder(params Expression<Func<TEntity, object>>[] includes)
             : this ()
        {
            _includes.AddRange(includes);
        }

        public void Add(Expression<Func<TEntity, object>> include)
        {
            _includes.Add(include);
        }

        public List<Expression<Func<TEntity, object>>> Get()
        {
            return _includes;
        }
    }
}
