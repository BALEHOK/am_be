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

        private static IndexEntity GetIndexEntity(IIndexEntity indexEntity)
        {
            object displayExtValues;
            object displayValues;
            try
            {
                displayExtValues = JsonConvert.DeserializeObject(indexEntity.DisplayExtValues);
            }
            catch
            {
                displayExtValues = indexEntity.DisplayExtValues;
            }
            try
            {
                displayValues = JsonConvert.DeserializeObject(indexEntity.DisplayValues);
            }
            catch
            {
                displayValues = indexEntity.DisplayValues;
            }

            return new IndexEntity
            {
                AllAttribValues = indexEntity.AllAttribValues,
                AllAttrib2IndexValues = indexEntity.AllAttrib2IndexValues,
                CategoryKeywords = indexEntity.CategoryKeywords,
                DynEntityConfigId = indexEntity.DynEntityConfigId,
                DynEntityId = indexEntity.DynEntityId,
                DynEntityUid = indexEntity.DynEntityUid,
                DisplayValues = displayValues,
                DisplayExtValues = displayExtValues
            };
        }
    }
}