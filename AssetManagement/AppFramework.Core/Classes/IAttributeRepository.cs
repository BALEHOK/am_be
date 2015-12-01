using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AppFramework.Entities;

namespace AppFramework.Core.Classes
{
    public interface IAttributeRepository
    {
        /// <summary>
        /// Returns Attrinute by ID where version is active - using cache
        /// </summary>
        DynEntityAttribConfig GetPublishedById(long id, Expression<Func<DynEntityAttribConfig, object>> include = null);

        IEnumerable<DynEntityAttribConfig> FindPublished(Expression<Func<DynEntityAttribConfig, bool>> filter, Expression<Func<DynEntityAttribConfig, object>> include = null);
        DynEntityAttribConfig FindPublishedSingleOrDefault(Expression<Func<DynEntityAttribConfig, bool>> filter, Expression<Func<DynEntityAttribConfig, object>> include = null);
    }
}