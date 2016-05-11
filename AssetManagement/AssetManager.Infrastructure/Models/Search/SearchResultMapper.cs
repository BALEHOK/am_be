using System;
using System.Collections.Generic;
using System.Linq;
using AppFramework.Entities;
using Newtonsoft.Json;
using Common.Logging;

namespace AssetManager.Infrastructure.Models.Search
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
            var item = new SearchResultItem
            {
                Name = indexEntity.Name,
                Description = indexEntity.Description,
                AllAttribValues = indexEntity.AllAttribValues,
                AllAttrib2IndexValues = indexEntity.AllAttrib2IndexValues,
                CategoryKeywords = indexEntity.CategoryKeywords,
                DynEntityConfigId = indexEntity.DynEntityConfigId,
                DynEntityId = indexEntity.DynEntityId,
                DynEntityUid = indexEntity.DynEntityUid,
            };

            try
            {
                item.DisplayValues = JsonConvert.DeserializeObject<KeyValuePair<string, string>[]>(
                    indexEntity.DisplayValues);
                item.DisplayExtValues = JsonConvert.DeserializeObject<KeyValuePair<string, string>[]>(
                    indexEntity.DisplayExtValues);
            }
            catch(JsonReaderException ex)
            {
                item.DisplayValues = new KeyValuePair<string, string>[] { };
                item.DisplayExtValues = new KeyValuePair<string, string>[] { };
                LogManager.GetCurrentClassLogger().Error("Cannot deserialize JSON", ex);
            }

            return item;
        }
    }
}