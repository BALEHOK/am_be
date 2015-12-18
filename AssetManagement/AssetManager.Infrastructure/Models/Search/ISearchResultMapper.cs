using System;
using System.Collections.Generic;
using AppFramework.Entities;

namespace AssetManager.Infrastructure.Models.Search
{
    public interface ISearchResultMapper
    {
        SearchResultModel GetSearchResultModel(Guid searchId, IEnumerable<IIndexEntity> indexEntities);
    }
}