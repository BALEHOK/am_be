namespace AppFramework.DataProxy
{
    using System;
    using System.Collections.Generic;
    using System.Data.Objects;
    using System.Linq;
    using System.Linq.Expressions;

    public interface IDataRepository<TEntity>
    {
        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Expression<Func<TEntity, object>> include = null,
            int? offset = null,
            int? pageSize = null);

        TEntity Single(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> include = null);

        List<TEntity> Where(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> include = null);

        List<TEntity> Where(Expression<Func<TEntity, bool>> filter, IEnumerable<Expression<Func<TEntity, object>>> includes);

        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> include = null);

        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> filter, IEnumerable<Expression<Func<TEntity, object>>> includes);

        TEntity Insert(TEntity entity);

        void Delete(TEntity entityToDelete);

        void Delete(List<TEntity> entitiesToDelete);

        TEntity Update(TEntity entityToUpdate);

        IQueryable<TEntity> AsQueryable();

        void LoadProperty(TEntity entity, Expression<Func<TEntity, object>> selector, MergeOption mergeOption = MergeOption.AppendOnly);
    }
}
