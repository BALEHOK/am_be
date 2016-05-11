using System;
using System.Collections.Generic;
using System.Linq;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.AC.Providers;
using AppFramework.Core.Calculation;
using AppFramework.Core.Classes;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.Core.DTO;
using AppFramework.Core.Exceptions;
using AppFramework.Core.Validation;
using AppFramework.DataProxy;
using AssetManager.Infrastructure.Helpers;
using AssetManager.Infrastructure.Models;
using Common.Logging;
using Newtonsoft.Json.Linq;
using AssetManager.Infrastructure.Permissions;

namespace AssetManager.Infrastructure.Services
{
    public class AssetService : IAssetService
    {
        private readonly IAssetsService _coreAssetsService;
        private readonly IDynamicListsService _dynListService;
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IAssetTypePermissionChecker _assetTypePermissionChecker;
        private readonly IValidationServiceNew _validationService;
        private readonly IModelFactory _modelFactory;
        private readonly IFileService _fileService;
        private readonly ILog _logger;
        private readonly IPasswordEncoder _passwordEncoder;
        private readonly IHistoryService _historyService;
        private readonly IAttributeCalculator _calculator;
        private readonly IUnitOfWork _unitOfWork;
        private long _userTypeId;
        private readonly IAssetPermissionChecker _assetPermissionChecker;

        // TODO: more than 7 dependencies, needs to be decomposed
        public AssetService(
            IAssetsService coreAssetsService,
            IDynamicListsService dynListService,
            IAssetTypeRepository assetTypeRepository,
            IAssetTypePermissionChecker assetTypePermissionChecker,
            IAssetPermissionChecker assetPermissionChecker,
            IValidationServiceNew validationService,
            IModelFactory modelFactory,
            IFileService fileService,
            IHistoryService historyService,
            IAttributeCalculator calculator,
            IUnitOfWork unitOfWork,
            ILog logger,
            IPasswordEncoder passwordEncoder)
        {
            if (coreAssetsService == null)
                throw new ArgumentNullException("coreAssetsService");
            _coreAssetsService = coreAssetsService;
            if (dynListService == null)
                throw new ArgumentNullException("dynListService");
            _dynListService = dynListService;
            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;
            if (assetTypePermissionChecker == null)
                throw new ArgumentNullException("assetTypePermissionChecker");
            _assetTypePermissionChecker = assetTypePermissionChecker;
            if (assetPermissionChecker == null)
                throw new ArgumentNullException("assetPermissionChecker");
            _assetPermissionChecker = assetPermissionChecker;
            if (validationService == null)
                throw new ArgumentNullException("validationService");
            _validationService = validationService;
            if (modelFactory == null)
                throw new ArgumentNullException("modelFactory");
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
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            if (logger == null)
                throw new ArgumentNullException("logger");
            _logger = logger;
            if (passwordEncoder == null)
                throw new ArgumentNullException("passwordEncoder");
            _passwordEncoder = passwordEncoder;
        }

        public long UserTypeId
        {
            get
            {
                if (_userTypeId == 0)
                {
                    var userTypeName = PredefinedEntity.User.ToString();
                    var attr = _unitOfWork.PredefinedAttributesRepository.Single(x => x.Name == userTypeName);
                    _userTypeId = attr.DynEntityConfigID;
                }

                return _userTypeId;
            }
        }

        public AssetModel GetAsset(
            long assetTypeId,
            long assetId,
            long userId,
            int? revision = null,
            long? uid = null,
            bool withChilds = false)
        {
            if (revision.HasValue && uid.HasValue)
                throw new ArgumentException("Cannot get asset by both revision and uid. Please pass one of those.");

            var assetType = _assetTypeRepository.GetById(assetTypeId);
            Asset asset;

            if (revision > 0)
                asset = _coreAssetsService.GetAssetByIdAndRevison(assetId, assetType, revision.Value);
            else if (uid > 0)
                asset = _coreAssetsService.GetAssetByUid(uid.Value, assetType);
            else
                asset = _coreAssetsService.GetAssetById(assetId, assetType);

            var permission = _assetPermissionChecker.GetPermission(asset, userId);
            if (!permission.CanRead())
                throw new InsufficientPermissionsException("No permissions to read asset's data");

            var assetWrapper = new AssetWrapperForScreenView(asset);
            var model = _modelFactory.GetAssetModel(assetWrapper, permission);

            if (withChilds && assetType.ParentChildRelations)
                model.ChildAssetTypes = GetRelatedAssetTypes(userId, assetTypeId);

            return model;
        }

        public AttributeModel GetAssetAttribute(long assetTypeId, long assetId, long attributeId, long userId)
        {
            var asset = GetAsset(assetTypeId, assetId, userId);
            return asset.Screens
                .SelectMany(s => s.Panels)
                .SelectMany(p => p.Attributes)
                .FirstOrDefault(a => a.Id == attributeId);
        }

        public void DeleteAsset(long assetTypeId, long assetId, long userId)
        {
            var assetType = _assetTypeRepository.GetById(assetTypeId);
            var asset = _coreAssetsService.GetAssetById(assetId, assetType);

            _assetPermissionChecker.EnsureDeletePermission(asset, userId);

            _coreAssetsService.DeleteAsset(asset);
        }

        public Tuple<long, string> SaveAsset(AssetModel model, long userId, long? screenId = null)
        {
            var assetType = _assetTypeRepository.GetById(model.AssetTypeId);
            Asset asset;

            if (model.Id != 0)
            {
                asset = _coreAssetsService.GetAssetById(model.Id, assetType);
                if (!asset.GetConfiguration().IsActiveVersion)
                    throw new Exception("Given AssetType Id refers to inactive version.");

                _assetPermissionChecker.EnsureWritePermission(asset, userId);
            }
            else
            {
                _assetTypePermissionChecker.EnsureWritePermission(assetType.Base, userId);

                asset = _coreAssetsService.CreateAsset(assetType);
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
                        assetType.ID, entityAttribute, modelAttribute.Value);
                    _modelFactory.AssignInternalAttributes(document, userId);
                    dependencies.Add(entityAttribute, document);
                }
                else if (entityAttribute.Configuration.DataTypeEnum == Enumerators.DataType.Password)
                {
                    var modelValue = modelAttribute.Value.Value<string>();

                    string password;
                    if (string.Equals(entityAttribute.Value, modelValue))
                    {
                        password = modelValue;
                    }
                    else
                    {
                        password = _passwordEncoder.EncodePassword(modelValue);

                        if (model.AssetTypeId == UserTypeId)
                        {
                            asset["LastActivityDate"].Value = DateTime.Now.ToString();
                        }
                    }

                    _modelFactory.AssignValueUnconditional(entityAttribute, password);
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

            var assetId = _coreAssetsService.InsertAsset(asset, dependencies).ID;
            return new Tuple<long, string>(assetId, asset.Name);
        }

        /// <summary>
        /// Use to update child asset only. not all the attributes are processed properly
        /// </summary>
        public Asset UpdateAsset(Dictionary<long, EntityAttribConfigModel> assetModel, long userId)
        {
            var entityConfigUid = 0L;
            var entityId = 0L;
            foreach (var attr in assetModel.Values)
            {
                if (attr.Name == AttributeNames.DynEntityConfigUid)
                {
                    entityConfigUid = attr.Value.Value<long>();
                }
                else if (attr.Name == AttributeNames.DynEntityId)
                {
                    entityId = attr.Value.Value<long>();
                }

                if (entityId > 0 && entityConfigUid > 0)
                {
                    break;
                }
            }

            var assetType = _assetTypeRepository.GetByUid(entityConfigUid);
            if (assetType == null)
            {
                throw new AssetTypeNotFoundException(null, entityConfigUid);
            }

            var asset = _coreAssetsService.GetAssetById(entityId, assetType);
            if (!asset.GetConfiguration().IsActiveVersion)
            {
                throw new Exception("Given AssetType Id refers to inactive version.");
            }

            _assetPermissionChecker.EnsureWritePermission(asset, userId);

            var dependencies = new Dictionary<AssetAttribute, Asset>();
            foreach (var modelAttribute in assetModel)
            {
                var entityAttribute = asset.Attributes.SingleOrDefault(
                    a => a.Configuration.ID == modelAttribute.Key);

                if (entityAttribute == null)
                    throw new Exception("Cannot bind model attribute to entity attribute");

                var valueObject = modelAttribute.Value.Value;
                _modelFactory.AssignValue(entityAttribute, valueObject);
            }

            _modelFactory.AssignInternalAttributes(asset, userId);

            var validationResult = _validationService.ValidateAsset(asset);
            if (!validationResult.IsValid)
            {
                throw new AssetValidationException(validationResult);
            }

            return _coreAssetsService.InsertAsset(asset, dependencies);
        }

        public IEnumerable<AssetAttributeRelatedEntitiesModel> GetAssetRelatedEntities(
            long assetTypeId, long? assetId, int? revision = null, long? uid = null)
        {
            if (revision.HasValue && uid.HasValue)
                throw new ArgumentException("Cannot get asset by both revision and uid. Please pass one of those.");

            var assetType = _assetTypeRepository.GetById(assetTypeId);
            Asset asset;

            if (assetId > 0 && revision > 0)
                asset = _coreAssetsService.GetAssetByIdAndRevison(assetId.Value, assetType, revision.Value);
            else if (uid > 0)
                asset = _coreAssetsService.GetAssetByUid(uid.Value, assetType);
            else if (assetId > 0)
                asset = _coreAssetsService.GetAssetById(assetId.Value, assetType);
            else
                asset = _coreAssetsService.CreateAsset(assetType);

            var result = new List<AssetAttributeRelatedEntitiesModel>();
            foreach (var attribute in asset.Attributes)
            {
                var model = new AssetAttributeRelatedEntitiesModel
                {
                    AttributeId = attribute.Configuration.ID,
                    AttributeUid = attribute.Configuration.UID,
                    Name = attribute.Configuration.NameLocalized,
                    Datatype = attribute.Configuration
                        .DataTypeEnum.ToString().ToLower()
                };

                switch (attribute.Configuration.DataTypeEnum)
                {
                    case Enumerators.DataType.Asset:
                        model.RelatedAssetTypeId = attribute.Configuration.RelatedAssetTypeID;
                        var relatedAsset = _coreAssetsService.GetRelatedAssetByAttribute(attribute);
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
                        var relatedAssets = _coreAssetsService.GetRelatedAssetsByAttribute(attribute);
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
            var revisions = _coreAssetsService
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
            var asset = _coreAssetsService
                .GetHistoryAssets(assetTypeId, assetId)
                .First();
            _coreAssetsService.RestoreAsset(asset);
        }

        public IEnumerable<AssetModel> GetAssetsByName(long assetTypeId, long userId, string query, int? rowStart,
            int? rowsNumber)
        {
            Func<Asset, bool> filterPredicate = (asset) =>
            {
                return query == null || asset.Name.ToLower().Contains(query.ToLower());
            };
            return GetAssets(assetTypeId, userId, filterPredicate, rowStart, rowsNumber);
        }

        public IEnumerable<AssetModel> GetAssets(long assetTypeId, long userId, Func<Asset, bool> filterPredicate = null,
            int? rowStart = null, int? rowsNumber = null)
        {
            var assets = _coreAssetsService.GetAssetsByAssetTypeIdAndUser(assetTypeId, userId, filterPredicate);

            var result = assets.Select(x => new AssetModel
            {
                AssetTypeId = assetTypeId,
                Name = x.Name,
                Id = x.ID
            });

            if (rowStart.HasValue)
            {
                result = result.Skip(rowStart.Value);
            }

            return rowsNumber.HasValue ? result.Take(rowsNumber.Value) : result;
        }

        public AssetModel CreateAsset(long assetTypeId, long userId)
        {
            var assetType = _assetTypeRepository.GetById(assetTypeId);
            _assetTypePermissionChecker.EnsureWritePermission(assetType.Base, userId);

            var asset = _coreAssetsService.CreateAsset(assetType); // TODO: get rid of this invocation
            asset[AttributeNames.UpdateUserId].ValueAsId = userId;
            asset[AttributeNames.UpdateDate].Value = DateTime.Now.ToString();
            var wrappedAsset = new AssetWrapperForScreenView(asset);
            _calculator.CalculateAssetScreens(wrappedAsset);

            return _modelFactory.GetAssetModel(wrappedAsset);
        }

        private Asset CreateRelatedEntity(long assetTypeId, AssetAttribute attribute, JToken value)
        {
            var document = _coreAssetsService.CreateAsset(PredefinedEntity.Document);
            document.Name = value["name"].Value<string>();
            var fileAttr = document.Attributes.FirstOrDefault(
                a => a.Configuration.DataTypeEnum == Enumerators.DataType.File);
            if (fileAttr == null)
                throw new EntityNotFoundException("Cannot find file attribute of Document asset type");

            var filename = value["name"].Value<string>();

            var targetFile = _fileService.MoveFileOnAssetCreation(
                assetTypeId,
                attribute.Configuration.ID,
                document.Configuration.ID,
                fileAttr.Configuration.ID,
                filename);

            fileAttr.Value = targetFile.Name;
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

        public AssetModel CalculateAsset(AssetModel model, long userId, long? screenId = null)
        {
            var assetType = _assetTypeRepository.GetById(model.AssetTypeId);
            Asset asset;

            if (model.Id != 0)
            {
                asset = _coreAssetsService.GetAssetById(model.Id, assetType);
                if (!asset.GetConfiguration().IsActiveVersion)
                    throw new Exception("Given AssetType Id refers to inactive version.");
            }
            else
            {
                asset = _coreAssetsService.CreateAsset(assetType);
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
            var assetWrapper = new AssetWrapperForScreenView(asset);
            _calculator.CalculateAssetScreens(assetWrapper, screenId);
            return _modelFactory.GetAssetModel(assetWrapper, Permission.RDRD);
        }

        public IEnumerable<ChildAssetType> GetRelatedAssetTypes(long userId, long assetTypeId)
        {
            var result = _unitOfWork.GetRelatedAssetTypes(userId, assetTypeId);
            return result.Select(i => new ChildAssetType
            {
                DynEntityConfigId = i.DynEntityConfigId,
                DynEntityAttribConfigId = i.DynEntityAttribConfigId,
                AssetTypeName = i.AssetTypeName,
                AttributeName = i.AttributeName
            });
        }
    }
}