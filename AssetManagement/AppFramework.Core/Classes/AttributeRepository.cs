using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AppFramework.DataProxy;
using AppFramework.Entities;
using LinqKit;

namespace AppFramework.Core.Classes
{
    public class AttributeRepository : IAttributeRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        private static readonly Expression<Func<DynEntityAttribConfig, bool>> PublishedAndAvailableExpr = attribute => attribute.ActiveVersion
                                                                                 && attribute.Active
                                                                                 && attribute.DynEntityConfig.Active;

        public AttributeRepository(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Returns Attrinute by ID where version is active - using cache
        /// </summary>
        public DynEntityAttribConfig GetPublishedById(long id, Expression<Func<DynEntityAttribConfig, object>> include = null)
        {
            return FindPublishedSingleOrDefault(a => a.DynEntityAttribConfigId == id, include);
        }

        public DynEntityAttribConfig FindPublishedSingleOrDefault(Expression<Func<DynEntityAttribConfig, bool>> filter, Expression<Func<DynEntityAttribConfig, object>> include = null)
        {
            var filterExpr = filter != null
                ? filter.And(PublishedAndAvailableExpr)
                : PublishedAndAvailableExpr;

            return _unitOfWork.DynEntityAttribConfigRepository
                .SingleOrDefault(filterExpr, include);
        }

        public IEnumerable<DynEntityAttribConfig> FindPublished(Expression<Func<DynEntityAttribConfig, bool>> filter, Expression<Func<DynEntityAttribConfig, object>> include = null)
        {
            var filterExpr = filter != null
                ? filter.And(PublishedAndAvailableExpr)
                : PublishedAndAvailableExpr;

            return _unitOfWork.DynEntityAttribConfigRepository
                .Get(filterExpr, include: include);
        }
    }
}