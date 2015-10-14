﻿using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AssetManager.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AssetManager.Infrastructure.Helpers;
using AppFramework.Core.DTO;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.Core.Classes.ScreensServices;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Exceptions;
using AppFramework.Core.Validation;
using System.ComponentModel.DataAnnotations;
using AppFramework.Core.Classes.Barcode;
using Newtonsoft.Json.Linq;
using System.Web.Hosting;
using System.IO;
using Common.Logging;
using AppFramework.Core.Calculation;

namespace AssetManager.Infrastructure.Services
{
    public class AssetService : IAssetService
    {
        private readonly IAssetsService _assetsService;
        private readonly IDynamicListsService _dynListService;
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly IValidationServiceNew _validationService;
        private readonly IModelFactory _modelFactory;
        private readonly IFileService _fileService;
        private readonly ILog _logger;
        private readonly IHistoryService _historyService;
        private readonly IAttributeCalculator _calculator;

        public AssetService(
            IAssetsService assetsService,
            IDynamicListsService dynListService,
            IAssetTypeRepository assetTypeRepository,
            IAuthenticationService authenticationService,
            IValidationServiceNew validationService,
            IModelFactory modelFactory,            
            IFileService fileService,
            IHistoryService historyService,
            IAttributeCalculator calculator,
            ILog logger)
        {
            if (assetsService == null)
                throw new ArgumentNullException("assetsService");
            _assetsService = assetsService;
            if (dynListService == null)
                throw new ArgumentNullException("dynListService");
            _dynListService = dynListService;
            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;
            if (authenticationService == null)
                throw new ArgumentNullException("authenticationService");
            _authenticationService = authenticationService;
            if (validationService == null)
                throw new ArgumentNullException("validationService");
            _validationService = validationService;
            if (modelFactory == null)
                throw new ArgumentNullException("attributeValueProvider");
            _modelFactory = modelFactory;           
            if (fileService == null)
                throw new ArgumentNullException("fileService");
            _fileService = fileService;
            if (historyService == null)
                throw new ArgumentNullException("historyService");
            _historyService = historyService;
            if (calculator == null)
                throw new ArgumentNullException("calculator");
            _calculator = calculator;
            if (logger == null)
                throw new ArgumentNullException("logger");
            _logger = logger;
        }

        public AssetModel GetAsset(long assetTypeId, long assetId, int? revision = null, long? uid = null)
        {
            if (revision.HasValue && uid.HasValue)
                throw new ArgumentException("Cannot get asset by both revision and uid. Please pass one of those.");

            var assetType = _assetTypeRepository.GetById(assetTypeId);
            AppFramework.Core.Classes.Asset asset;

            if (revision > 0)
                asset = _assetsService.GetAssetByIdAndRevison(assetId, assetType, revision.Value);
            else if (uid > 0)
                asset = _assetsService.GetAssetByUid(uid.Value, assetType);
            else
                asset = _assetsService.GetAssetById(assetId, assetType);

            var permission = _authenticationService.GetPermission(asset);
            if (!permission.CanRead())
                throw new InsufficientPermissionsException("No permissions to read asset's data");
           
            return _modelFactory.GetAssetModel(asset, permission);
        }

        public AttributeModel GetAssetAttribute(long assetTypeId, long assetId, long attributeId)
        {
            var asset = GetAsset(assetTypeId, assetId);
            return asset.Screens
                .SelectMany(s => s.Panels)
                .SelectMany(p => p.Attributes)
                .SingleOrDefault(a => a.Id == attributeId);
        }

        public void DeleteAsset(long assetTypeId, long assetId)
        {
            var assetType = _assetTypeRepository.GetById(assetTypeId);
            var asset = _assetsService.GetAssetById(assetId, assetType);
            var permission = _authenticationService.GetPermission(asset);
            _assetsService.DeleteAsset(asset, permission);
        }

        public Tuple<long, string> SaveAsset(AssetModel model, long userId, long? screenId = null)
        {
            var assetType = _assetTypeRepository.GetById(model.AssetTypeId);
            Asset asset;

            if (model.Id != 0)
            {
                asset = _assetsService.GetAssetById(model.Id, assetType);
                if (!asset.GetConfiguration().IsActiveVersion)
                    throw new Exception("Given AssetType Id refers to inactive version.");
            }
            else
            {
                asset = _assetsService.CreateAsset(assetType);
            }

            var dependencies = new Dictionary<AssetAttribute, Asset>();
            foreach (var modelAttribute in model.GetAttributes(screenId))
            {
                var entityAttribute = asset.Attributes.SingleOrDefault(
                    a => a.Configuration.ID == modelAttribute.Id);

                if (entityAttribute == null)
                    throw new Exception("Cannot bind model attribute to entity attribute");

                if (IsNewDocument(modelAttribute, entityAttribute))
                {
                    var document = CreateRelatedEntity(
                        assetType.ID, entityAttribute, modelAttribute.Value, dependencies);
                    _modelFactory.AssignInternalAttributes(document, userId);
                    dependencies.Add(entityAttribute, document);
                }
                else
                {
                    _modelFactory.AssignValue(
                        entityAttribute, modelAttribute.Value);
                }
            }

            _modelFactory.AssignInternalAttributes(asset, userId);

            var validationResult = _validationService.ValidateAsset(asset);
            if (!validationResult.IsValid)
                throw new AssetValidationException(validationResult);

            var assetId = _assetsService.InsertAsset(asset, dependencies).ID;
            return new Tuple<long, string>(assetId, asset.Name);
        }
                
        public IEnumerable<AssetAttributeRelatedEntitiesModel> GetAssetRelatedEntities(
            long assetTypeId, long? assetId, int? revision = null, long? uid = null)
        {
            if (revision.HasValue && uid.HasValue)
                throw new ArgumentException("Cannot get asset by both revision and uid. Please pass one of those.");

            var assetType = _assetTypeRepository.GetById(assetTypeId);
            AppFramework.Core.Classes.Asset asset;

            if (assetId > 0 && revision > 0)
                asset = _assetsService.GetAssetByIdAndRevison(assetId.Value, assetType, revision.Value);
            else if (uid > 0)
                asset = _assetsService.GetAssetByUid(uid.Value, assetType);
            else if (assetId > 0)
                asset = _assetsService.GetAssetById(assetId.Value, assetType);
            else
                asset = _assetsService.CreateAsset(assetType);

            var result = new List<AssetAttributeRelatedEntitiesModel>();
            foreach (var attribute in asset.Attributes)
            {
                var model = new AssetAttributeRelatedEntitiesModel
                {
                    AttributeId = attribute.Configuration.ID,
                    AttributeUid = attribute.Configuration.UID,
                    Name = attribute.Configuration.Name,
                    Datatype = attribute.Configuration
                        .DataTypeEnum.ToString().ToLower()
                };

                switch (attribute.Configuration.DataTypeEnum)
                {
                    case Enumerators.DataType.Asset:
                        model.RelatedAssetTypeId = attribute.Configuration.RelatedAssetTypeID;
                        var relatedAsset = _assetsService.GetRelatedAssetByAttribute(attribute);
                        if (relatedAsset != null)
                        {
                            model.Assets = new List<PlainAssetDTO>
                            {
                                new PlainAssetDTO
                                {
                                    Id = relatedAsset.ID,
                                    Name = relatedAsset.GetDisplayName(
                                        attribute.Configuration.RelatedAssetTypeAttributeID.Value),
                                    AssetTypeId = attribute.Configuration.RelatedAssetTypeID.Value,
                                    Revision = relatedAsset.Revision
                                }
                            };
                        }
                        result.Add(model);
                        break;

                    case Enumerators.DataType.Assets:
                        model.RelatedAssetTypeId = attribute.Configuration.RelatedAssetTypeID;
                        var relatedAssets = _assetsService.GetRelatedAssetsByAttribute(attribute);
                        model.Assets =
                            from a in relatedAssets
                            select a;
                        result.Add(model);
                        break;

                    case Enumerators.DataType.DynList:
                    case Enumerators.DataType.DynLists:
                        var list = _dynListService.GetByUid(attribute.Configuration.DynamicListUid.Value);
                        model.List = list.ToDto();
                        result.Add(model);
                        break;
                }                
            }
            return result;
        }

        public AssetHistoryModel GetAssetHistory(long assetTypeId, long assetId)
        {
            var revisions = _assetsService
                .GetHistoryAssets(assetTypeId, assetId)
                .ToList();
            var assetTypeUser = _assetTypeRepository.GetPredefinedAssetType(
                PredefinedEntity.User);

            var model = new AssetHistoryModel
            {
                Revisions = (from revision in revisions
                             let prevRevision = revisions.SingleOrDefault(r =>
                                 r.Revision == revision.Revision - 1)
                             select new AssetRevisionModel
                             {
                                 RevisionNumber = revision[AttributeNames.Revision].Value,
                                 CreatedAt = revision[AttributeNames.UpdateDate].Value,
                                 //AssetTypeUid = asset[AttributeNames.DynEntityConfigUid].Value,
                                 AssetTypeId = revision.Configuration.ID,
                                 AssetId = revision.ID,
                                 CreatedByUser = new PlainAssetDTO 
                                 {
                                     Name = revision[AttributeNames.UpdateUserId].Value,
                                     Id = revision[AttributeNames.UpdateUserId].ValueAsId.Value,
                                     AssetTypeId = assetTypeUser.ID
                                 },
                                 ChangedValues = _historyService.GetChangesBetweenRevisions(
                                    revision, prevRevision).ToList()
                             })
                            .ToList()
            };
            return model;
        }       
        
        public void RestoreAsset(long assetTypeId, long assetId)
        {
            var asset = _assetsService
                .GetHistoryAssets(assetTypeId, assetId)
                .First();
            _assetsService.RestoreAsset(asset);
        }

        public IEnumerable<AssetModel> GetAssets(long assetTypeId, long userId, string query, int? rowStart, int? rowsNumber)
        {
            var assets = _assetsService.GetAssetsByAssetTypeIdAndUser(
                assetTypeId, userId, rowStart, rowsNumber);

            return from asset in assets
                   where query == null || asset.Name.ToLower().Contains(query.ToLower())
                   select new AssetModel
                   {
                       AssetTypeId = assetTypeId,
                       Name = asset.Name,
                       Id = asset.ID,
                   };
        }

        public AssetModel CreateAsset(long assetTypeId, long userId)
        {
            var assetType = _assetTypeRepository.GetById(assetTypeId);
            var asset = _assetsService.CreateAsset(assetType); // TODO: get rid of this invocation
            asset[AttributeNames.UpdateUserId].ValueAsId = userId;
            asset[AttributeNames.UpdateDate].Value = DateTime.Now.ToString();
            asset = _calculator.PreCalculateAsset(asset);
            return _modelFactory.GetAssetModel(asset);
        }

        private Asset CreateRelatedEntity(
            long assetTypeId,
            AssetAttribute attribute,
            JToken value,
            IDictionary<AssetAttribute, Asset> dependencies)
        {
            var document = _assetsService.CreateAsset(PredefinedEntity.Document);
            document.Name = value["name"].Value<string>();
            var fileAttr = document.Attributes.FirstOrDefault(
                a => a.Configuration.DataTypeEnum == Enumerators.DataType.File);
            if (fileAttr == null)
                throw new EntityNotFoundException("Cannot find file attribute of Document asset type");

            var filename = value["name"].Value<string>();

            var targetPath = _fileService.MoveFileOnAssetCreation(
                assetTypeId, 
                attribute.Configuration.ID, 
                document.Configuration.ID,
                fileAttr.Configuration.ID, 
                filename);

            fileAttr.Value = Path.GetFileName(targetPath);
            return document;
        }

        private static bool IsNewDocument(AttributeModel modelAttribute, AssetAttribute entityAttribute)
        {
            return entityAttribute.Configuration.DataTypeEnum == Enumerators.DataType.Document &&
                modelAttribute.Value.HasValues &&
                (modelAttribute.Value["id"].Type == JTokenType.Null ||
                modelAttribute.Value["id"].Value<long>() == 0) &&
                modelAttribute.Value["name"].Type == JTokenType.String &&
                !string.IsNullOrEmpty(modelAttribute.Value["name"].Value<string>());
        }

        public AssetModel CalculateAsset(AssetModel model, long userId, long? screenId = null, bool overwrite = false)
        {
            var assetType = _assetTypeRepository.GetById(model.AssetTypeId);
            Asset asset;

            if (model.Id != 0)
            {
                asset = _assetsService.GetAssetById(model.Id, assetType);
                if (!asset.GetConfiguration().IsActiveVersion)
                    throw new Exception("Given AssetType Id refers to inactive version.");
            }
            else
            {
                asset = _assetsService.CreateAsset(assetType);
            }

            foreach (var modelAttribute in model.GetAttributes(screenId))
            {
                var entityAttribute = asset.Attributes.SingleOrDefault(
                    a => a.Configuration.ID == modelAttribute.Id);

                if (entityAttribute == null)
                    throw new Exception("Cannot bind model attribute to entity attribute");

                _modelFactory.AssignValue(
                        entityAttribute, modelAttribute.Value);
            }

            _modelFactory.AssignInternalAttributes(asset, userId);

            var validationResult = _validationService.ValidateAsset(asset);
            if (!validationResult.IsValid)
                throw new AssetValidationException(validationResult);

            asset = _calculator.PreCalculateAsset(asset, screenId, overwrite);
            return _modelFactory.GetAssetModel(asset, Permission.RDRD);
        }
    }
}