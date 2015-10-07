namespace AppFramework.DataProxy
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Objects;
    using System.Linq;
    using System.Linq.Expressions;
    using AppFramework.DataLayer;
    using AppFramework.Entities;
    using LinqKit;

    /// <summary>
    /// DataRepository class which provides the interface to work with strongly-typed EF entities
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DataRepository<TEntity> : IDataRepository<TEntity> where TEntity : class, AppFramework.Entities.IDataEntity, IObjectWithChangeTracker
    {
        protected ObjectSet<TEntity> _objectSet;
        protected ObjectContext _context;

        public DataRepository(ObjectContext context)
        {
            _context = context;
            _objectSet = _context.CreateObjectSet<TEntity>();
        }

        public IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Expression<Func<TEntity, object>> include = null,
            int? offset = null,
            int? pageSize = null)
        {
            IQueryable<TEntity> query = include != null ?
                           _objectSet.Include(include) : _objectSet;

            if (filter != null)
            {
                query = query.AsExpandable().Where(filter);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (offset.HasValue && pageSize.HasValue)
            {
                query = query.Skip(offset.Value).Take(pageSize.Value);
            }
            return query.ToList();
        }

        public TEntity Single(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> include = null)
        {
            var entity = SingleOrDefault(filter, include);
            if (entity == null)
                throw new System.Exception("Sequence contains no elements");
            return entity;
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> include = null)
        {
            IQueryable<TEntity> entitySet = include != null ?
               _objectSet.Include(include) : _objectSet;
            return SingleOrDefault(entitySet, filter);
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> filter, IEnumerable<Expression<Func<TEntity, object>>> includes)
        {
            IQueryable<TEntity> entitySet = _objectSet;
            foreach (var include in includes)
            {
                entitySet = entitySet.Include(include);
            }
            return SingleOrDefault(entitySet, filter);
        }

        public TEntity Insert(TEntity entity)
        {
            var fqen = GetEntityName();
            _context.AddObject(fqen, entity);
            _context.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
            return entity;
        }

        public void Delete(TEntity entityToDelete)
        {
            _context.DeleteObject(entityToDelete);
            _context.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
        }

        public TEntity Update(TEntity entityToUpdate)
        {
            var fqen = GetEntityName();
            _context.ApplyChanges<TEntity>(fqen, entityToUpdate);

            //EntityKey key = _context.CreateEntityKey(fqen, entityToUpdate);
            //object original = null;
            //if (_context.TryGetObjectByKey(key, out original))
            //    //_context.ApplyCurrentValues(fqen, entityToUpdate);
            //    _context.ApplyChanges<TEntity>(fqen, entityToUpdate);
            //else
            //    throw new ObjectNotFoundException();
            return entityToUpdate;
        }

        public IQueryable<TEntity> AsQueryable()
        {
            return _objectSet;
        }

        /// <summary>
        /// Loads the collection of child entities
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="selector"></param>
        /// <param name="mergeOption"></param>
        public void LoadProperty(TEntity entity, Expression<Func<TEntity, object>> selector, MergeOption mergeOption = MergeOption.NoTracking)
        {
            var fqen = GetEntityName();
            AttachIfDetached(entity, fqen);
            _context.LoadProperty(entity, selector, mergeOption);
        }

        #region Extensibility methods
        protected IEnumerable<TEntity> Get(IQueryable<TEntity> entitySet, Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        {
            IQueryable<TEntity> query = entitySet;

            if (filter != null)
            {
                query = query.AsExpandable().Where(filter);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            //if (offset.HasValue && pageSize.HasValue)
            //{
            //    query = query.Skip(offset.Value).Take(pageSize.Value);
            //}
            return query.ToList();
        }

        protected TEntity Single(IQueryable<TEntity> entitySet, Expression<Func<TEntity, bool>> filter)
        {
            var entity = SingleOrDefault(entitySet, filter);
            if (entity == null)
                throw new System.Exception("Sequence contains no elements");
            return entity;
        }

        protected TEntity SingleOrDefault(IQueryable<TEntity> entitySet, Expression<Func<TEntity, bool>> filter)
        {
            return entitySet.AsExpandable()
                            .Where(filter)
                            .ToList()
                            .SingleOrDefault();
        }

        #endregion

        private string GetEntityName()
        {
            return string.Format("{0}.{1}", _objectSet.EntitySet.EntityContainer, _objectSet.EntitySet.Name);
        }

        private void AttachIfDetached(TEntity entity, string fqen)
        {
            EntityKey key = _context.CreateEntityKey(fqen, entity);
            ObjectStateEntry ose = null;
            if (_context.ObjectStateManager.TryGetObjectStateEntry(key, out ose) && ose.State != System.Data.EntityState.Detached)
            {
                // no need to attach the object
            }
            else
            {
                _context.AttachTo(fqen, entity);
            }
        }
    
        public void Detach(object entity)
        {
            _context.Detach(entity);
        }


        public List<TEntity> Where(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> include = null)
        {
            IQueryable<TEntity> entitySet = include != null ?
              _objectSet.Include(include) : _objectSet;
            return entitySet.AsExpandable()
                            .Where(filter)
                            .ToList();
        }

        public List<TEntity> Where(Expression<Func<TEntity, bool>> filter, IEnumerable<Expression<Func<TEntity, object>>> includes)
        {
            IQueryable<TEntity> entitySet = _objectSet;
            foreach (var include in includes)
            {
                entitySet = entitySet.Include(include);
            }
            return entitySet.AsExpandable()
                            .Where(filter)
                            .ToList();
        }

        public void Delete(List<TEntity> entitiesToDelete)
        {
            foreach (var item in entitiesToDelete)
            {
                Delete(item);
            }
        }
    }
}
