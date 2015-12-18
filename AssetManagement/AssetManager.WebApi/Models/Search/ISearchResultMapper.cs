using System;
using System.Collections.Generic;
using AppFramework.Entities;
using AssetManager.Infrastructure.Models;

namespace AssetManager.WebApi.Models.Search
{
    public interface ISearchResultMapper
    {
        SearchResultModel GetSearchResultModel(Guid searchId, IEnumerable<IIndexEntity> indexEntities);
    }
}