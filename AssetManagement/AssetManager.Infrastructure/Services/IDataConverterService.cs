using System;
using System.Collections.Generic;
using AppFramework.Entities;
using AssetManager.Infrastructure.Models;

namespace AssetManager.Infrastructure.Services
{
    public interface IDataConverterService
    {
        SearchResultModel GetSearchResultModel(Guid searchId, IEnumerable<IIndexEntity> indexEntities);
    }
}