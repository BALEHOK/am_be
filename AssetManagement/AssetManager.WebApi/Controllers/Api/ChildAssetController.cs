using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.Extensions;
using AppFramework.Core.Classes.ScreensServices;
using AppFramework.Core.Exceptions;
using AppFramework.Core.Services;
using AppFramework.Entities;
using AssetManager.Infrastructure.Extensions;
using AssetManager.Infrastructure.Models;
using AssetManager.Infrastructure.Permissions;
using AssetManager.Infrastructure.Services;
using Newtonsoft.Json.Linq;

namespace AssetManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/asset")]
    public class ChildAssetController : ApiController
    {
        private readonly IPanelsService _panelService;
        private readonly IAttributeRepository _attributeRepository;
        private readonly IAssetsService _assetsService;
        private readonly IAssetService _assetService;
        private readonly IDynamicListsService _dynamicListsService;
        private readonly IAssetTypePermissionChecker _assetTypePermissionChecker;

        public ChildAssetController(
            IPanelsService panelService,
            IAttributeRepository attributeRepository,
            IAssetsService assetsService,
            IAssetService assetService,
            IDynamicListsService dynamicListsService,
            IAssetTypePermissionChecker assetTypePermissionChecker)
        {
            if (panelService == null)
                throw new ArgumentNullException("panelService");
            _panelService = panelService;

            if (attributeRepository == null)
                throw new ArgumentNullException("attributeRepository");
            _attributeRepository = attributeRepository;

            if (assetsService == null)
                throw new ArgumentNullException("assetsService");
            _assetsService = assetsService;

            if (assetService == null)
                throw new ArgumentNullException("assetService");
            _assetService = assetService;

            if (dynamicListsService == null)
                throw new ArgumentNullException("dynamicListsService");
            _dynamicListsService = dynamicListsService;

            if (assetTypePermissionChecker == null)
                throw new ArgumentNullException("assetTypePermissionChecker");
            _assetTypePermissionChecker = assetTypePermissionChecker;
        }

        [HttpPut]
        [Route("")]
        public Dictionary<long, EntityAttribConfigModel> Update(Dictionary<long, EntityAttribConfigModel> assetModel)
        {
            var asset = _assetService.UpdateAsset(assetModel, User.GetId());

            return AssetToModel(asset);
        }

        [HttpGet]
        [Route("{assetId}/panel/{panelId}/childAssets")]
        public object Get(long assetId, long panelId)
        {
            var panel = _panelService.GetById(panelId);

            if (panel == null || !panel.IsChildAssets || !panel.ChildAssetAttrId.HasValue)
            {
                throw new EntityNotFoundException("Child assets panel with given Id doesn't exists or not accessible.");
            }

            var childAttrId = panel.ChildAssetAttrId.Value;
            var childEntityAttrib = _attributeRepository.FindPublishedSingleOrDefault(
                attr => attr.DynEntityAttribConfigId == childAttrId,
                attr => attr.DynEntityConfig);

            var childAssetTypeId = childEntityAttrib.DynEntityConfig.DynEntityConfigId;
            var userId = User.GetId();

            var assets = _assetsService
                .GetAssetsByAssetTypeIdAndUser(
                    childAssetTypeId,
                    userId,
                    a =>
                    {
                        var relationAttr = a.Attributes.SingleOrDefault(attr => attr.Configuration.ID == childAttrId);
                        return relationAttr != null && relationAttr.ValueAsId.GetValueOrDefault() == assetId;
                    })
                .ToList();

            var panelModel = new
            {
                Id = panel.AttributePanelId,
                Name = panel.Name,
                IsChildAssets = panel.IsChildAssets,
                Editable = _assetTypePermissionChecker.GetPermission(childAssetTypeId, userId).CanWrite(),
                ChildAssetTypeId = childAssetTypeId,
                ChildAssetAttrId = panel.ChildAssetAttrId.GetValueOrDefault(),
                Attributes = panel.AttributePanelAttribute.Select(apa => ConfigAttributeToModel(userId, apa.DynEntityAttribConfig))
            };

            return new
            {
                Panel = panelModel,
                Assets = assets.Select(AssetToModel)
            };
        }

        private object ConfigAttributeToModel(long userId, DynEntityAttribConfig attr)
        {
            return new
            {
                Id = attr.DynEntityAttribConfigId,
                Datatype = attr.DataType.Name,
                attr.Name,
                Label = attr.NameLocalized(),
                Editable = attr.DataType.IsEditable && attr.AllowEditValue,
                CanCreateNew = attr.RelatedAssetTypeID.HasValue && _assetTypePermissionChecker.GetPermission(attr.RelatedAssetTypeID.Value, userId).CanWrite(),
                Required = attr.IsRequired,
                RelatedAssetTypeId = attr.RelatedAssetTypeID
            };
        }

        private Dictionary<long, EntityAttribConfigModel> AssetToModel(Asset asset)
        {
            var assetAttributes = asset.Attributes
                .ToDictionary(attr => attr.Configuration.ID, AssetAttributeToModel);

            return assetAttributes;
        }

        private EntityAttribConfigModel AssetAttributeToModel(AssetAttribute attr)
        {
            var model = new EntityAttribConfigModel
            {
                Name = attr.Configuration.Name
            };

            switch (attr.Configuration.DataTypeEnum)
            {
                case Enumerators.DataType.Bool:
                    model.Value = new JValue(bool.Parse(attr.Value));
                    break;

                case Enumerators.DataType.Asset:
                    model.Value = new JObject(
                            new JProperty("id", new JValue(attr.ValueAsId.GetValueOrDefault())),
                            new JProperty("name", new JValue(attr.Value)));
                    break;

                case Enumerators.DataType.DynList:
                    var listValue = _dynamicListsService.GetListValuesByAttribute(attr).FirstOrDefault();
                    if (listValue != null)
                    {
                        model.Value = new JValue(listValue.Value);
                    }
                    break;

                default:
                    model.Value = new JValue(attr.Value);
                    break;
            }

            return model;
        }
    }
}