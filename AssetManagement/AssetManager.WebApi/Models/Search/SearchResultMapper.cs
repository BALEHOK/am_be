using System;
using System.Collections.Generic;
using System.Linq;
using AppFramework.Entities;
using AssetManager.Infrastructure.Models;
using Newtonsoft.Json;

namespace AssetManager.WebApi.Models.Search
{
    public class SearchResultMapper : ISearchResultMapper
    {
        public SearchResultModel GetSearchResultModel(Guid searchId, IEnumerable<IIndexEntity> indexEntities)
        {
            var entities = indexEntities.Select(GetIndexEntity).ToList();

            return new SearchResultModel
            {
                SearchId = searchId,
                Entities = entities
            };
        }

        private static SearchResultItem GetIndexEntity(IIndexEntity indexEntity)
        {
            return new SearchResultItem
            {
                AllAttribValues = indexEntity.AllAttribValues,
                AllAttrib2IndexValues = indexEntity.AllAttrib2IndexValues,
                CategoryKeywords = indexEntity.CategoryKeywords,
                DynEntityConfigId = indexEntity.DynEntityConfigId,
                DynEntityId = indexEntity.DynEntityId,
                DynEntityUid = indexEntity.DynEntityUid,
                DisplayValues = JsonConvert.DeserializeObject(indexEntity.DisplayValues),
                DisplayExtValues = JsonConvert.DeserializeObject(indexEntity.DisplayExtValues)
            };
        }
    }
}