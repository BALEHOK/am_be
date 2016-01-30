using System;
using System.Collections.Generic;
using System.Linq;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.Barcode;
using AppFramework.Core.Classes.DynLists;
using AppFramework.Core.Classes.ScreensServices;
using AppFramework.Core.ConstantsEnumerators;
using AssetManager.Infrastructure.Helpers;
using AssetManager.Infrastructure.Models;
using AssetManager.Infrastructure.Services;
using Newtonsoft.Json.Linq;

namespace AssetManager.Infrastructure
{
    public class ModelFactory : IModelFactory
    {
        private readonly IAttributeValueFormatter _attributeValueFormatter;
        private readonly IDynamicListsService _dynamicListsService;
        private readonly IAssetsService _assetsService;
        private readonly IAssetPanelsAdapter _panelsAdapter;
        private readonly IScreensService _screensService;
        private readonly IEnvironmentSettings _envSettings;
        private readonly IBarcodeProvider _barcodeProvider;

        public ModelFactory(
            IAttributeValueFormatter attributeValueFormatter,
            IDynamicListsService dynamicListsService,
            IAssetsService assetsService,
            IScreensService screensService,
            IAssetPanelsAdapter panelsAdapter,
            IEnvironmentSettings envSettings,
            IBarcodeProvider barcodeProvider)
        {
            if (attributeValueFormatter == null)
                throw new ArgumentNullException("attributeValueFormatter");
            _attributeValueFormatter = attributeValueFormatter;
            if (dynamicListsService == null)
                throw new ArgumentNullException("dynamicListsService");
            _dynamicListsService = dynamicListsService;
            if (assetsService == null)
                throw new ArgumentNullException("assetsService");
            _assetsService = assetsService;
            if (screensService == null)
                throw new ArgumentNullException("screensService");
            _screensService = screensService;
            if (panelsAdapter == null)
                throw new ArgumentNullException("panelsAdapter");
            _panelsAdapter = panelsAdapter;
            if (envSettings == null)
                throw new ArgumentNullException("envSettings");
            _envSettings = envSettings;
            if (barcodeProvider == null)
                throw new ArgumentNullException("barcodeProvider");
            _barcodeProvider = barcodeProvider;
        }

        public AttributeModel GetAttributeModel(AssetAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException("attribute");

            var attributeConfig = attribute.Configuration;
            var attributeData = attribute.Data;
            var isActive = !attribute.ParentAsset.IsHistory;
            var assetUid = attribute.ParentAsset.UID;

            var model = new AttributeModel
            {
                Uid = attributeConfig.UID,
                Id = attributeConfig.ID,
                Name = attributeConfig.Name,
                Datatype = attributeConfig.DataTypeEnum.ToString().ToLower(),
                Editable = attributeConfig.Editable,
                Required = attributeConfig.IsRequired
            };

            if (attributeConfig.DataTypeEnum == Enumerators.DataType.Asset ||
                attributeConfig.DataTypeEnum == Enumerators.DataType.Assets ||
                attributeConfig.DataTypeEnum == Enumerators.DataType.Document)
                model.RelatedAssetTypeId = attributeConfig.RelatedAssetTypeID;

            var displayValue = _attributeValueFormatter.GetDisplayValue(
                attributeConfig,
                attributeData.Value,
                isActive);

            switch (attributeConfig.DataTypeEnum)
            {
                case Enumerators.DataType.Asset:
                case Enumerators.DataType.Document:
                case Enumerators.DataType.Role:

                    if (attributeData.Value == null ||
                        string.IsNullOrEmpty(attributeData.Value.ToString()))
                    {
                        model.Value = new JObject(
                            new JProperty("id", JValue.CreateNull()),
                            new JProperty("name", JValue.CreateNull()));
                    }
                    else
                    {
                        model.Value = new JObject(
                            new JProperty("id", attributeData.Value),
                            new JProperty("name", displayValue));
                    }
                    break;

                case Enumerators.DataType.Assets:
                    var assets = _assetsService.GetRelatedAssetsByAttribute(attribute);
                    model.Value = new JArray(
                        assets.Select(
                            asset => new JObject(
                                new JProperty("id", asset.Id),
                                new JProperty("name", asset.Name)))
                            .ToArray());
                    break;

                case Enumerators.DataType.DynList:
                case Enumerators.DataType.DynLists:
                    var listValues = _dynamicListsService.GetLegacyListValues(attributeConfig, assetUid).ToList();
                    model.Value = new JObject(
                        new JProperty("id", string.Join(",", listValues.Select(lv => lv.DynamicListItemId).Take(1))),
                        new JProperty("value", _attributeValueFormatter.GetDisplayValue(listValues)));
                    break;

                case Enumerators.DataType.Zipcode:
                case Enumerators.DataType.Place:
                    model.Value = new JObject(
                        new JProperty("id", attributeData.Value),
                        new JProperty("name", attributeData.Value));
                    break;

                case Enumerators.DataType.Image:
                    if (!string.IsNullOrEmpty(attributeData.Value.ToString()))
                    {
                        var relPath = _envSettings.GetAssetMediaRelativePath(
                            attribute.ParentAsset.Configuration.ID,
                            attributeConfig.ID);
                        model.Value = string.Format("{0}/{1}/{2}",
                            _envSettings.GetAssetMediaHttpRoot(), relPath, attributeData.Value);
                    }
                    break;

                case Enumerators.DataType.DateTime:
                case Enumerators.DataType.CurrentDate:
                    model.Value = new JValue(attributeData.Value);
                    break;

                default:
                    model.Value = new JValue(displayValue);
                    break;
            }

            return model;
        }

        public void AssignValue(AssetAttribute attribute, JToken value)
        {
            if (attribute == null)
                throw new ArgumentNullException("attribute");

            if (value == null)
            {
                attribute.Value = null;
                return;
            }

            switch (attribute.Configuration.DataTypeEnum)
            {
                case Enumerators.DataType.Asset:
                case Enumerators.DataType.Document:
                    if (value.HasValues &&
                        value.Type == JTokenType.Object &&
                        value["id"].Type == JTokenType.Integer)
                        attribute.ValueAsId = value["id"].Value<long>();
                    break;

                case Enumerators.DataType.Role:
                    if (value.HasValues &&
                        value.Type == JTokenType.Object &&
                        value["id"].Type == JTokenType.Integer)
                        attribute.ValueAsId = value["id"].Value<long>();
                    break;

                case Enumerators.DataType.Zipcode:
                case Enumerators.DataType.Place:
                    if (value.HasValues)
                        attribute.Value = value["name"].Value<string>();
                    break;

                case Enumerators.DataType.Assets:
                    long[] ids = null;
                    if (value.HasValues && value.Type == JTokenType.Array)
                    {
                        ids = value.Select(v => v.Value<long>("id")).ToArray();
                    }
                    else if (value.HasValues && value.Type == JTokenType.Object &&
                             value["id"].Type == JTokenType.Integer)
                    {
                        ids = new[] {value["id"].Value<long>()};
                    }

                    if (ids != null)
                    {
                        attribute.Value = string.Join(",", ids);
                        attribute.MultipleAssets.Clear();
                        attribute.MultipleAssets.AddRange(
                            ids.Select(id => new KeyValuePair<long, string>(id, string.Empty)));
                    }
                    break;

                case Enumerators.DataType.DynList:
                    if (value.HasValues && value["id"].Type == JTokenType.Integer)
                    {
                        var listItemId = value["id"].Value<long>();
                        var listItem = _dynamicListsService.GetListItemById(listItemId);
                        var listValue = new DynamicListValue
                        {
                            DynamicListItemUid = listItem.DynListItemUid,
                            Value = listItem.Value
                        };
                        attribute.AddDynamicListValue(listValue);
                    }
                    break;

                case Enumerators.DataType.DynLists:
                    //throw new NotImplementedException("DynLists aren't supported yet");
                    break;

                case Enumerators.DataType.CurrentDate:
                    attribute.Value = DateTime.Now.ToString(
                        ApplicationSettings.DisplayCultureInfo.DateTimeFormat);
                    break;

                case Enumerators.DataType.Image:
                    var imageName = value.Value<string>();
                    if (!string.IsNullOrEmpty(imageName))
                        attribute.Value = imageName
                            .Split('/', '\\')
                            .Last();
                    break;

                default:
                    attribute.Value = value.Value<string>();
                    break;
            }
        }

        public void AssignValueUnconditional(AssetAttribute attribute, string value)
        {
            if (attribute == null)
                throw new ArgumentNullException("attribute");

            if (value == null)
            {
                attribute.Value = null;
                return;
            }

            attribute.Value = value;
        }

        public AssetModel GetAssetModel(Asset asset, Permission? permission = null)
        {
            if (asset == null)
                throw new ArgumentNullException("asset");

            var barcodeAttr = asset[AttributeNames.Barcode];
            if (barcodeAttr != null && asset.IsNew)
                barcodeAttr.Value = _barcodeProvider.GenerateBarcode();

            var model = new AssetModel
            {
                AssetTypeId = asset.GetConfiguration().ID,
                Id = asset.ID,
                Name = asset.Name,
                IsHistory = asset.IsHistory,
                IsDeleted = asset.IsDeleted,
                Revision = asset.Revision,
                UpdatedAt = asset.UpdatedAt,
                Screens = _getAssetScreens(asset),
                Editable = permission.HasValue
                    ? permission.Value.CanWrite()
                    : true,
                Deletable = permission.HasValue
                    ? permission.Value.CanDelete()
                    : true,
                Barcode = asset.Barcode
            };
            return model;
        }

        public void AssignInternalAttributes(Asset asset, long userId)
        {
            asset[AttributeNames.ActiveVersion].Value = true.ToString();
            asset[AttributeNames.UpdateUserId].ValueAsId = userId;
            asset[AttributeNames.UpdateDate].Value = DateTime.Now.ToString(
                ApplicationSettings.DisplayCultureInfo.DateTimeFormat);
        }

        private IEnumerable<AssetScreenModel> _getAssetScreens(Asset asset)
        {
            var screens = _screensService.GetScreensByAssetTypeUid(asset.Configuration.UID);
            return from s in screens
                select new AssetScreenModel
                {
                    Id = s.ScreenId,
                    Name = s.Name,
                    IsDefault = s.IsDefault,
                    IsMobile = s.IsMobile,
                    LayoutType = s.ScreenLayout.Type,
                    Panels = _panelsAdapter.GetPanelsByScreen(asset, s)
                        .ToPanelModels(GetAttributeModel),
                    HasFormula = asset
                        .Configuration
                        .Panels
                        .Where(p => p.ScreenId == s.ScreenId)
                        .Any(p => p.Base.AttributePanelAttribute
                            .Any(apa => apa.ScreenFormula != null))
                };
        }
    }
}