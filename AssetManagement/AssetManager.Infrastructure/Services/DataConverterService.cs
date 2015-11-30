using System;
using System.Collections.Generic;
using System.Linq;
using AppFramework.Entities;
using AssetManager.Infrastructure.Models;
using Newtonsoft.Json;

namespace AssetManager.Infrastructure.Services
{
    public class DataConverterService : IDataConverterService
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
                AllContextAttribValues = indexEntity.AllContextAttribValues,
                BarCode = indexEntity.BarCode,
                CategoryKeywords = indexEntity.CategoryKeywords,
                Department = indexEntity.Department,
                DepartmentId = indexEntity.DepartmentId,
                Description = indexEntity.Description,
                DynEntityConfigId = indexEntity.DynEntityConfigId,
                DynEntityConfigUid = indexEntity.DynEntityConfigUid,
                DynEntityId = indexEntity.DynEntityId,
                DynEntityUid = indexEntity.DynEntityUid,
                EntityConfigKeywords = indexEntity.EntityConfigKeywords,
                Keywords = indexEntity.Keywords,
                Location = indexEntity.Location,
                LocationUid = indexEntity.LocationUid,
                Name = indexEntity.Name,
                OwnerId = indexEntity.OwnerId,
                Subtitle = indexEntity.Subtitle,
                TaxonomyItemsIds = indexEntity.TaxonomyItemsIds,
                TaxonomyKeywords = indexEntity.TaxonomyKeywords,
                UpdateDate = indexEntity.UpdateDate,
                User = indexEntity.User,
                UserId = indexEntity.UserId,
                DisplayValues = displayValues,
                DisplayExtValues = displayExtValues
            };
        }
    }
}