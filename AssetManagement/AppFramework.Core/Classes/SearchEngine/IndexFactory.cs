using AppFramework.DataProxy;
using Newtonsoft.Json;

namespace AppFramework.Core.Classes.SearchEngine
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using AppFramework.ConstantsEnumerators;
    using AppFramework.Core.ConstantsEnumerators;
    using AppFramework.Core.Helpers;
    using AppFramework.Entities;

    class IndexFactory<T>
        where T : class, Entities.IDataEntity, new()
    {
        public static IEnumerable<T> GenerateIndexEntities(Asset asset)
        {
            if (typeof (T) == typeof (DynEntityContextAttributesValues))
            {
                var dyncontextvalues = new TrackableCollection<DynEntityContextAttributesValues>();
                foreach (
                    AssetAttribute attribute in asset.Attributes.Where(a => a.GetConfiguration().ContextId.HasValue &&
                                                                            !string.IsNullOrWhiteSpace(a.Value)))
                {
                    var value = new DynEntityContextAttributesValues()
                    {
                        ContextId = attribute.GetConfiguration().ContextId.Value,
                        DynEntityUid = asset.UID,
                        DynEntityConfigUid = asset.GetConfiguration().UID,
                        IsActive = !asset.IsHistory
                    };

                    switch (attribute.Configuration.DataTypeEnum)
                    {
                        case Enumerators.DataType.DynList:
                            foreach (var dynlistitem in attribute.DynamicListValues)
                            {
                                dyncontextvalues.Add(new DynEntityContextAttributesValues
                                {
                                    ContextId = attribute.GetConfiguration().ContextId.Value,
                                    DynEntityUid = asset.UID,
                                    DynEntityConfigUid = asset.GetConfiguration().UID,
                                    IsActive = !asset.IsHistory,
                                    DynamicListItemUid = dynlistitem.DynamicListItemUid
                                });
                            }
                            break;
                        case Enumerators.DataType.Long:
                        case Enumerators.DataType.Int:
                        case Enumerators.DataType.Revision:
                        case Enumerators.DataType.Role:
                        case Enumerators.DataType.Zipcode:
                        case Enumerators.DataType.Permission:
                            value.NumericValue = long.Parse(attribute.Value);
                            dyncontextvalues.Add(value);
                            break;
                        case Enumerators.DataType.Asset:
                        case Enumerators.DataType.Assets:
                            value.NumericValue = attribute.ValueAsId != null
                                ? attribute.ValueAsId.Value
                                : 0;
                            dyncontextvalues.Add(value);
                            break;
                        case Enumerators.DataType.Float:
                            double resultvalue;
                            if (double.TryParse(attribute.Value, NumberStyles.Float,
                                ApplicationSettings.DisplayCultureInfo, out resultvalue))
                            {
                                value.NumericValue = resultvalue;
                            }
                            else
                            {
                                value.StringValue = attribute.Value;
                            }
                            dyncontextvalues.Add(value);
                            break;
                        case Enumerators.DataType.DateTime:
                        case Enumerators.DataType.CurrentDate:
                            if (!string.IsNullOrEmpty(attribute.Value))
                            {
                                DateTime dt;
                                if (DateTime.TryParse(attribute.Value,
                                    ApplicationSettings.DisplayCultureInfo.DateTimeFormat, DateTimeStyles.None, out dt))
                                    value.DateTimeValue = dt;
                            }
                            else
                            {
                                value.StringValue = attribute.Value;
                            }
                            dyncontextvalues.Add(value);
                            break;
                        default:
                            value.StringValue = attribute.Value;
                            dyncontextvalues.Add(value);
                            break;
                    }
                }
                return dyncontextvalues.OfType<T>();
            }
            throw new ArgumentException(string.Format("Generation of collection is not supported for type {0}",
                typeof (T).Name));
        }

        public static T GenerateIndexEntity(Asset asset,
            IUnitOfWork unitOfWork,
            IAssetTypeRepository assetTypeRepository,
            IAssetsService assetsService)
        {
            if (typeof (T) != typeof (IndexActiveDynEntities) && (typeof (T) != typeof (IndexHistoryDynEntities)))
                throw new ArgumentException(string.Format("Index generation for type {0} not supported", typeof (T).Name));

            AssetType assetType = asset.GetConfiguration();
            var taxItemsInfo = AssetFactory.GetTaxonomyItemsInformation(asset.UID, asset.GetConfiguration().UID);

            HashSet<string>[] indexes =
                getIndexes(asset.AttributesPublic, assetTypeRepository, assetsService, isIndexRelatedAssets: true);
            string keywords = Formatting.FormatForSearch(string.Join(" ", indexes[0]));
            string fullIndex = Formatting.FormatForSearch(string.Join(" ", indexes[1]));
            string allAttributes = Formatting.FormatForSearch(string.Join(" ", indexes[2]));
            string withContext = Formatting.FormatForSearch(string.Join(" ", indexes[3]));
            string desc = Formatting.FormatForSearch(string.Join(" ", indexes[4]));
            string contextName = string.Empty;
            if (assetType.ContextId != null)
            {
                var context = unitOfWork.ContextRepository
                    .SingleOrDefault(c => c.ContextId == (long) assetType.ContextId);
                contextName = context == null
                    ? string.Empty
                    : context.Name;
            }

            var shortDetailsArray = from attribute in asset.Attributes
                                    where attribute.GetConfiguration().DisplayOnResultList
                                          && !string.IsNullOrWhiteSpace(attribute.Value)
                                    orderby attribute.GetConfiguration().DisplayOrderExtResultList
                                    select new KeyValuePair<string, string>(attribute.GetConfiguration().Name,
                                        attribute.GetConfiguration().DataTypeEnum == Enumerators.DataType.DateTime ?
                                            attribute.GetValueAsDateTime().ToString("dd/MM/yyyy") : attribute.Value.Trim());

            var extendedDetailsArray = from attribute in asset.Attributes
                                       where attribute.GetConfiguration().DisplayOnExtResultList
                                               && !string.IsNullOrWhiteSpace(attribute.Value)
                                       orderby attribute.GetConfiguration().DisplayOrderExtResultList
                                       select new KeyValuePair<string, string>(attribute.GetConfiguration().Name,
                                           attribute.GetConfiguration().DataTypeEnum == Enumerators.DataType.DateTime ?
                                               attribute.GetValueAsDateTime().ToString("dd/MM/yyyy") : attribute.Value.Trim());

            var entity = (IIndexEntity) Activator.CreateInstance<T>();
            entity.DynEntityUid = asset.UID;
            entity.DynEntityConfigUid = asset.DynEntityConfigUid;
            entity.BarCode = asset[AttributeNames.Barcode] != null ? asset[AttributeNames.Barcode].Value : string.Empty;
            entity.Name = asset.Name;
            entity.Description = desc;
            entity.Keywords = keywords;
            entity.EntityConfigKeywords = assetType.NameInvariant + " " + contextName;
            entity.AllAttrib2IndexValues = fullIndex;
            entity.AllContextAttribValues = withContext;
            entity.AllAttribValues = allAttributes;
            entity.CategoryKeywords = taxItemsInfo.CategoriesDescriptions;
            entity.TaxonomyKeywords = taxItemsInfo.TaxonomiesDescriptions;
            entity.TaxonomyItemsIds = taxItemsInfo.TaxonomyItemsIds;
            entity.User = asset[AttributeNames.UserId] != null ? asset[AttributeNames.UserId].Value : string.Empty;
            entity.Location = asset[AttributeNames.LocationId] != null
                ? asset[AttributeNames.LocationId].Value
                : string.Empty;
            entity.Department = asset[AttributeNames.DepartmentId] != null
                ? asset[AttributeNames.DepartmentId].Value
                : string.Empty;
            entity.DepartmentId = (asset[AttributeNames.DepartmentId] != null &&
                                   asset[AttributeNames.DepartmentId].ValueAsId.HasValue)
                ? asset[AttributeNames.DepartmentId].ValueAsId.Value
                : default(long);
            entity.OwnerId = (asset[AttributeNames.OwnerId] != null && asset[AttributeNames.OwnerId].ValueAsId.HasValue)
                ? asset[AttributeNames.OwnerId].ValueAsId.Value
                : default(long);
            entity.UserId = (asset[AttributeNames.UserId] != null) ? asset[AttributeNames.UserId].ValueAsId : null;
            entity.UpdateDate = !string.IsNullOrEmpty(asset[AttributeNames.UpdateDate].Value)
                ? Convert.ToDateTime(asset[AttributeNames.UpdateDate].Value,
                    ApplicationSettings.DisplayCultureInfo.DateTimeFormat)
                : DateTime.Now;
            entity.DynEntityConfigId = assetType.ID;
            entity.DynEntityId = asset.ID;
            entity.DisplayValues = JsonConvert.SerializeObject(shortDetailsArray);
            entity.DisplayExtValues = JsonConvert.SerializeObject(extendedDetailsArray);

            if (asset[AttributeNames.LocationId] != null
                && asset[AttributeNames.LocationId].RelatedAsset != null)
                entity.LocationUid = asset[AttributeNames.LocationId].RelatedAsset.UID;
            return entity as T;
        }

        /// <summary>
        /// indexes[0] = keywords;
        /// indexes[1] = fullIndex;
        /// indexes[2] = allAttributes;
        /// indexes[3] = withContext;
        /// indexes[4] = desc;
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="isIndexRelatedAssets"></param>
        /// <returns></returns>
        private static HashSet<string>[] getIndexes(
            IEnumerable<AssetAttribute> attributes, 
            IAssetTypeRepository assetTypeRepository,
            IAssetsService assetsService,
            bool isIndexRelatedAssets)
        {
            HashSet<string> keywords = new HashSet<string>();
            HashSet<string> fullIndex = new HashSet<string>();
            HashSet<string> allAttributes = new HashSet<string>();
            HashSet<string> withContext = new HashSet<string>();
            HashSet<string> desc = new HashSet<string>();

            foreach (AssetAttribute attribute in attributes)
            {
                // skipping attributes which should not be in index
                if (attribute.GetConfiguration().DataTypeEnum == Enumerators.DataType.DateTime ||
                    attribute.GetConfiguration().DataTypeEnum == Enumerators.DataType.CurrentDate ||
                    attribute.GetConfiguration().DataTypeEnum == Enumerators.DataType.Bool ||
                    attribute.GetConfiguration().DataTypeEnum == Enumerators.DataType.Password ||
                    attribute.GetConfiguration().DataTypeEnum == Enumerators.DataType.Revision ||
                    attribute.GetConfiguration().DataTypeEnum == Enumerators.DataType.Permission ||
                    attribute.GetConfiguration().IsIdentity)
                {
                    continue;
                }

                if (isIndexRelatedAssets && (attribute.GetConfiguration().IsAsset || attribute.GetConfiguration().IsMultipleAssets))
                {
                    var allRelatedAttributes = new List<AssetAttribute>();
                    if (attribute.GetConfiguration().IsAsset)
                    {
                        if (attribute.RelatedAsset != null)
                        {
                            allRelatedAttributes = attribute.RelatedAsset.AttributesPublic;
                        }
                        else
                        {
                            //throw new NullReferenceException(string.Format("Cannot load related asset. Asset Id: {0}, Asset type Id: {1}",
                            //    attribute.ValueAsID, attribute.GetConfiguration().RelatedAssetTypeID));
                        }
                    }
                    else
                    {
                        if (attribute.MultipleAssets != null)
                        {
                            try
                            {
                                allRelatedAttributes = (from relatedAsset in attribute.MultipleAssets
                                                        let at = assetTypeRepository.GetById(attribute.GetConfiguration().RelatedAssetTypeID.Value)
                                                        select assetsService.GetAssetById(relatedAsset.Key, at)
                                                                 .AttributesPublic)
                                                                 .SelectMany(s => s)
                                                                 .ToList();
                            }
                            catch
                            {
                                //throw new NullReferenceException("Cannot load one of multiple assets: " + ex.Message);
                            }
                        }
                    }

                    if (attribute.GetConfiguration().IsKeyword)
                    {
                        getIndexes(allRelatedAttributes, assetTypeRepository, assetsService, false)[0].ForEachWithIndex((item, index) => keywords.Add(item));
                    }
                    if (attribute.GetConfiguration().IsFullIndex)
                    {
                        getIndexes(allRelatedAttributes, assetTypeRepository, assetsService, false)[1].ForEachWithIndex((item, index) => fullIndex.Add(item));
                    }
                    getIndexes(allRelatedAttributes, assetTypeRepository, assetsService, false)[2].ForEachWithIndex((item, index) => allAttributes.Add(item));
                    if (attribute.IsWithContext)
                    {
                        getIndexes(allRelatedAttributes, assetTypeRepository, assetsService, false)[3].ForEachWithIndex((item, index) => withContext.Add(item));
                    }
                    if (attribute.GetConfiguration().IsDescription)
                    {
                        getIndexes(allRelatedAttributes, assetTypeRepository, assetsService, false)[4].ForEachWithIndex((item, index) => desc.Add(item));
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(attribute.Value))
                        continue;

                    string[] attributeWords = attribute.Value.Split(new char[] { ' ', '/' });
                    if (attribute.GetConfiguration().IsKeyword)
                    {
                        attributeWords.ForEachWithIndex((item, index) => keywords.Add(item));
                    }
                    if (attribute.GetConfiguration().IsFullIndex)
                    {
                        attributeWords.ForEachWithIndex((item, index) => fullIndex.Add(item));
                    }
                    attributeWords.ForEachWithIndex((item, index) => allAttributes.Add(item));
                    if (attribute.IsWithContext)
                    {
                        attributeWords.ForEachWithIndex((item, index) => withContext.Add(item));
                    }
                    if (attribute.GetConfiguration().IsDescription)
                    {
                        attributeWords.ForEachWithIndex((item, index) => desc.Add(item));
                    }
                }
            }

            HashSet<string>[] indexes = new HashSet<string>[5];
            indexes[0] = keywords;
            indexes[1] = fullIndex;
            indexes[2] = allAttributes;
            indexes[3] = withContext;
            indexes[4] = desc;

            return indexes;
        }
    }
}
