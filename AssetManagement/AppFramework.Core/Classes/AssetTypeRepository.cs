using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes.Barcode;
using AppFramework.Core.Classes.Caching;
using AppFramework.Core.Classes.IE;
using AppFramework.Core.Classes.Validation;
using AppFramework.Core.DAL;
using AppFramework.Core.DAL.Adapters;
using AppFramework.Core.Exceptions;
using AppFramework.Core.Interceptors;
using AppFramework.Core.Properties;
using AppFramework.Core.Services;
using AppFramework.Core.Validation;
using AppFramework.DataProxy;
using AppFramework.Entities;
using AutoMapper;
using LinqKit;

namespace AppFramework.Core.Classes
{
    public class AssetTypeRepository : IAssetTypeRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationRulesService _validationRulesService;
        private readonly IDynColumnAdapter _dynColumnAdapter;
        private readonly IAssetTypeTaxonomyManager _assetTypeTaxonomyManager;
        private readonly IRightsService _rightsService;

        public AssetTypeRepository(
            IUnitOfWork unitOfWork,
            IValidationRulesService validationRulesService,
            IDynColumnAdapter dynColumnAdapter,
            IAssetTypeTaxonomyManager assetTypeTaxonomyManager,
            IRightsService rightsService)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            if (dynColumnAdapter == null)
                throw new ArgumentNullException("dynColumnAdapter");
            _dynColumnAdapter = dynColumnAdapter;
            if (validationRulesService == null)
                throw new ArgumentNullException("validationRulesService");
            _validationRulesService = validationRulesService;
            if (assetTypeTaxonomyManager == null)
                throw new ArgumentNullException("assetTypeTaxonomyManager");
            _assetTypeTaxonomyManager = assetTypeTaxonomyManager;
            if (rightsService == null)
                throw new ArgumentNullException("rightsService");
            _rightsService = rightsService;
        }

        [Obsolete(
            "This method will be removed in the next version. Use dependency injection to provide IAssetTypeRepository instance for your class."
            )]
        public static AssetTypeRepository Create(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");

            var dataTypeService = new DataTypeService(unitOfWork);
            var dynColumnAdapter = new DynColumnAdapter(dataTypeService);
            var validationRulesService = new ValidationRulesService(unitOfWork,
                new ValidationOperatorFactory(unitOfWork, new DefaultBarcodeProvider()));
            var assetTypeTaxonomyManager = new AssetTypeTaxonomyManager(unitOfWork);
            var rightsService = new RightsService(unitOfWork);
            return new AssetTypeRepository(unitOfWork,
                validationRulesService,
                dynColumnAdapter,
                assetTypeTaxonomyManager,
                rightsService);
        }

        /// <summary>
        /// Gets the assetType by UID
        /// </summary>
        /// <param name="uid">The uid.</param>
        /// <param name="activeOnly"></param>
        /// <returns></returns>
        public AssetType GetByUid(long uid, bool activeOnly = true)
        {
            var ibuilder = new IncludesBuilder<DynEntityConfig>();
            ibuilder.Add(e => e.DynEntityAttribConfigs.Select(a => a.DataType));
            ibuilder.Add(e => e.AttributePanel
                .Select(a => a.AttributePanelAttribute
                    .Select(apa => apa.DynEntityAttribConfig)));
            var data = _unitOfWork.DynEntityConfigRepository
                .SingleOrDefault(
                    d => d.DynEntityConfigUid == uid && (d.Active || !activeOnly),
                    ibuilder.Get());

            return data != null
                ? new AssetType(data, _unitOfWork)
                : null;
        }

        /// <summary>
        /// Returns AssetType by ID where version is active - using cache
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="activeOnly"></param>
        /// <returns></returns>
        public AssetType GetById(long id, bool activeOnly = true)
        {
            AssetType assetType;
            if (!TryGetById(id, out assetType, activeOnly))
            {
                throw new AssetTypeNotFoundException(id, null);
            }

            return assetType;
        }

        public bool TryGetById(long id, out AssetType assetType, bool activeOnly = true)
        {
            var ibuilder = new IncludesBuilder<DynEntityConfig>();
            ibuilder.Add(e => e.DynEntityAttribConfigs
                .Select(a => a.DataType.ValidationList
                    .Select(l => l.ValidationOperator)));
            ibuilder.Add(e => e.AttributePanel
                .Select(ap => ap.AttributePanelAttribute
                    .Select(a => a.DynEntityAttribConfig)));
            var data = _unitOfWork.DynEntityConfigRepository
                .SingleOrDefault(
                    dec =>
                        dec.DynEntityConfigId == id
                        && dec.ActiveVersion
                        && (dec.Active || !activeOnly),
                    ibuilder.Get());

            if (data == null)
            {
                assetType = null;
                return false;
            }

            assetType = new AssetType(data, _unitOfWork);
            return true;
        }

        /// <summary>
        /// Gets the type of the general asset - which contain only predefined attributes
        /// </summary>
        /// <returns></returns>
        public AssetType GetGeneralAssetType()
        {
            var at = ImportExportManager.GetBasicAssetTypeConfiguration(
                Enumerators.AssetTypeClass.NormalAssetType, _unitOfWork, new LayoutRepository(_unitOfWork));
            return at;
        }

        public Dictionary<long, AssetType> GetTypeRevisions(long typeId)
        {
            var includes = new IncludesBuilder<DynEntityConfig>();
            includes.Add(e => e.DynEntityAttribConfigs.Select(a => a.DataType));

            var typeRevisions = _unitOfWork.DynEntityConfigRepository
                .Where(c => c.DynEntityConfigId == typeId, includes.Get())
                .ToDictionary(c => c.DynEntityConfigUid, c => new AssetType(c, _unitOfWork));

            return typeRevisions;
        }

        public IEnumerable<AssetType> GetAllReservable()
        {
            var predicate = PredicateBuilder.True<DynEntityConfig>();
            predicate = predicate.And(dec => dec.ActiveVersion);
            predicate = predicate.And(dec => dec.Active);
            predicate = predicate.And(dec => dec.AllowBorrow);
            predicate = predicate.And(dec => dec.IsUnpublished == false);
            return _unitOfWork.DynEntityConfigRepository
                .Get(predicate, entities => entities.OrderBy(e => e.Name))
                .Select(item => new AssetType(item, _unitOfWork));
        }

        /// <summary>
        /// Returns recent AssetTypes
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AssetType> GetRecent()
        {
            var predicate = PredicateBuilder.True<DynEntityConfig>();
            predicate = predicate.And(dec => dec.ActiveVersion);
            predicate = predicate.And(dec => dec.Active);
            predicate = predicate.And(dec => dec.IsUnpublished == false);
            foreach (var item in _unitOfWork.DynEntityConfigRepository
                .Get(predicate, items => items.OrderByDescending(i => i.UpdateDate)).Take(10))
            {
                yield return new AssetType(item, _unitOfWork);
            }
        }

        /// <summary>
        /// Returns the list of all AssetTypes with active revision.
        /// </summary>
        /// <returns>List of AssetType objects</returns>
        public IEnumerable<AssetType> GetAllPublished()
        {
            var predicate = PredicateBuilder.True<DynEntityConfig>();
            predicate = predicate.And(dec => dec.ActiveVersion);
            predicate = predicate.And(dec => dec.Active);
            predicate = predicate.And(dec => dec.IsUnpublished == false);
            return _unitOfWork.DynEntityConfigRepository
                .Get(predicate, items => items.OrderBy(i => i.Name))
                .Select(item => new AssetType(item, _unitOfWork));
        }

        /// <summary>
        /// Saves complete AssetType definition into database. 
        /// Incrementing the revision number.
        /// </summary>
        [Transaction]
        public void Save(AssetType assetType, long currentUserId, List<TaxonomyContainer> containers)
        {
            var isNew = assetType.ID == 0;
            var updateDate = DateTime.Now;
            var entityConfig = assetType.Base;
            entityConfig.UpdateUserId = currentUserId;
            entityConfig.UpdateDate = updateDate;
            entityConfig.ActiveVersion = entityConfig.DynEntityConfigId == 0;
            entityConfig.Revision++;

            var userContext = _unitOfWork.ContextRepository.SingleOrDefault(c => c.Name == "User");
            var ownerContext = _unitOfWork.ContextRepository.SingleOrDefault(c => c.Name == "Owner");

            foreach (var attribute in entityConfig.DynEntityAttribConfigs)
            {
                attribute.ActiveVersion = entityConfig.DynEntityConfigId == 0;
                attribute.Revision++;
                attribute.UpdateUserId = currentUserId;
                attribute.UpdateDate = updateDate;
                if (attribute.ContextId.HasValue)
                    continue;
                if (attribute.Name.ToLower() == "user" && userContext != null)
                    attribute.ContextId = userContext.ContextId;
                if (attribute.Name.ToLower() == "owner" && ownerContext != null)
                    attribute.ContextId = ownerContext.ContextId;
            }

            foreach (var panel in entityConfig.AttributePanel)
            {
                panel.UpdateUserId = currentUserId;
                panel.UpdateDate = updateDate;

                foreach (var apa in panel.AttributePanelAttribute)
                {
                    apa.UpdateUserId = currentUserId;
                    apa.UpdateDate = updateDate;
                }
            }

            foreach (var screen in entityConfig.AssetTypeScreen)
            {
                screen.UpdateUserId = currentUserId;
                screen.UpdateDate = updateDate;
            }

            var revision = CreateRevision(assetType, currentUserId);

            using (var scope = new TransactionScope())
            {
                // Create table if new AssetType
                _unitOfWork.DynEntityConfigRepository.Insert(revision);

                // if new type - set it's ID equal to UID
                if (revision.DynEntityConfigId == 0)
                    revision.DynEntityConfigId = revision.DynEntityConfigUid;

                assetType.Base = revision;

                // save related taxonomies
                if (containers != null)
                {
                    if (!assetType.IsUnpublished)
                        _assetTypeTaxonomyManager.PersistContainers(revision.DynEntityConfigId, containers);
                    else
                        _assetTypeTaxonomyManager.SerializeContainers(revision.DynEntityConfigId, containers);
                }

                if (isNew)
                {
                    var table = new DynTable(_dynColumnAdapter, _unitOfWork);
                    table.Create(assetType);
                }
                scope.Complete();
            }

            // TODO : nobody will ever understand this, refactor the whole validation system

            foreach (var attribute in assetType.Attributes)
            {
                var validationRules = _validationRulesService.GetValidationRulesForAttribute(attribute).ToList();
                if (!validationRules.Any())
                    continue;

                List<DynEntityAttribValidation> validationLists = null;
                if (attribute.Base.DynEntityAttribConfigId != 0)
                {
                    var dynEntityAttrConfigId = attribute.Base.DynEntityAttribConfigId;
                    validationLists = _unitOfWork.DynEntityAttribValidationRepository
                        .Get(deav => deav.DynEntityAttribConfigId == dynEntityAttrConfigId,
                            include: deav => deav.ValidationList)
                        .Select(av => av)
                        .ToList();
                }

                foreach (var rule in validationRules)
                {
                    if (rule.ValidationList.ChangeTracker.State == ObjectState.Modified)
                        _unitOfWork.ValidationListRepository.Update(rule.ValidationList);
                    else if (rule.ValidationList.ChangeTracker.State == ObjectState.Added)
                        _unitOfWork.ValidationListRepository.Insert(rule.ValidationList);

                    var attributeValidation =
                        _unitOfWork.DynEntityAttribValidationRepository.SingleOrDefault(deav =>
                            deav.ValidationUid == rule.ValidationList.ValidationUid
                            && deav.DynEntityAttribConfigId == attribute.Base.DynEntityAttribConfigId);
                    if (attributeValidation == null)
                    {
                        var configId = attribute.Base.DynEntityAttribConfigId != 0
                            ? attribute.Base.DynEntityAttribConfigId
                            : assetType.Base.DynEntityAttribConfigs.FirstOrDefault(
                                a => a.DBTableFieldname == attribute.Base.DBTableFieldname).DynEntityAttribConfigUid;
                        _unitOfWork.DynEntityAttribValidationRepository.Insert(
                            new DynEntityAttribValidation
                            {
                                DynEntityAttribConfigId = configId,
                                ValidationUid = rule.ValidationList.ValidationUid
                            });
                    }
                    else
                    {
                        DynEntityAttribValidation vitem;
                        if (validationLists != null &&
                            (vitem =
                                validationLists.FirstOrDefault(
                                    v => v.ValidationUid == attributeValidation.ValidationUid)) != null)
                        {
                            validationLists.Remove(vitem);
                        }
                    }

                    foreach (var op in rule.ValidationOperator.Operands)
                    {
                        var vovEntity = _unitOfWork.ValidationOperandValueRepository.SingleOrDefault(v =>
                            v.ValidationListUid == rule.ValidationList.ValidationUid &&
                            v.ValidationOperandUid == op.Base.OperandUid);
                        if (vovEntity == null)
                        {
                            _unitOfWork.ValidationOperandValueRepository.Insert(new ValidationOperandValue
                            {
                                Value = op.Value.ToString(),
                                ValidationListUid = rule.ValidationList.ValidationUid,
                                ValidationOperandUid = op.Base.OperandUid
                            });
                        }
                        else if (op.Value != null)
                        {
                            vovEntity.Value = op.Value.ToString();
                            _unitOfWork.ValidationOperandValueRepository.Update(vovEntity);
                        }
                    }
                }
                if (validationLists != null)
                {
                    foreach (var item in validationLists)
                    {
                        _unitOfWork.ValidationListRepository.Delete(item.ValidationList);
                    }
                }
            }
            _unitOfWork.Commit();

            if (isNew)
            {
                _rightsService.SetPermissionsForUser(
                    new[] {new RightsEntry {AssetTypeID = assetType.ID, Permission = Permission.ReadWriteDelete}},
                    currentUserId,
                    currentUserId);
            }
        }

        private static DynEntityConfig CreateRevision(AssetType assetType, long currentUserId)
        {
            var _base = assetType.Base;
            var revision = new DynEntityConfig();
            Mapper.Map(_base, revision);

            if (_base.DynEntityAttribConfigs.Count == 0)
                throw new Exception("DynEntityAttribConfig property not loaded");

            // create attributes revisions
            for (var i = 0; i < _base.DynEntityAttribConfigs.Count; i++)
            {
                var attributeRevision = new DynEntityAttribConfig();
                Mapper.Map(_base.DynEntityAttribConfigs[i], attributeRevision);
                revision.DynEntityAttribConfigs.Add(attributeRevision);
            }

            if (_base.AttributePanel.Count == 0)
                throw new Exception("AttributePanel property not loaded");

            // create panels revisions
            for (var i = 0; i < _base.AttributePanel.Count; i++)
            {
                var panelRevision = new AttributePanel();
                Mapper.Map(_base.AttributePanel[i], panelRevision);

                // create attributes to panels revisions
                for (var j = 0; j < _base.AttributePanel[i].AttributePanelAttribute.Count; j++)
                {
                    var panel = assetType.Panels[i];
                    var referentialAttribute = panel.AssignedAttributes.ElementAtOrDefault(j);
                    if (referentialAttribute == null)
                        continue;

                    var apaRevision = new AttributePanelAttribute
                    {
                        UpdateUserId = currentUserId,
                        UpdateDate = DateTime.Now,
                        DisplayOrder = panel.GetAttributeDisplayOrder(referentialAttribute),
                        ScreenFormula = panel.GetAttributeScreenFormula(referentialAttribute)
                    };

                    var attributeConfig = (from attribute in revision.DynEntityAttribConfigs
                        // zero for a new attribute or should be equals before revision commit
                        where attribute.DynEntityConfigUid == referentialAttribute.AssetTypeUID &&
                              ((attribute.DynEntityAttribConfigUid != 0 &&
                                referentialAttribute.UID == attribute.DynEntityAttribConfigUid) ||
                               (attribute.DBTableFieldname == referentialAttribute.DBTableFieldName))
                        select attribute)
                        .SingleOrDefault();

                    if (attributeConfig != null)
                    {
                        // revision of asset's attribute
                        apaRevision.DynEntityAttribConfig = attributeConfig;
                    }
                    else // it's related asset type attribute
                    {
                        // related asset's attribute stays unchanged, so just copy its uid
                        apaRevision.DynEntityAttribConfigUId = referentialAttribute.UID;
                    }
                    apaRevision.ReferencingDynEntityAttribConfigId =
                        _base.AttributePanel[i].AttributePanelAttribute[j].ReferencingDynEntityAttribConfigId;
                    panelRevision.AttributePanelAttribute.Add(apaRevision);
                }
                revision.AttributePanel.Add(panelRevision);
            }

            if (ApplicationSettings.ApplicationType == ApplicationType.SOBenBUB ||
                ApplicationSettings.ApplicationType == ApplicationType.Combined)
            {
                // create screens revisions
                for (var i = 0; i < _base.AssetTypeScreen.Count; i++)
                {
                    var screenRevision = new AssetTypeScreen();
                    Mapper.Map(_base.AssetTypeScreen[i],
                        screenRevision);

                    // update connections between screens and panels
                    for (var j = 0; j < revision.AttributePanel.Count; j++)
                    {
                        if (revision.AttributePanel[j].ScreenId == _base.AssetTypeScreen[i].ScreenId)
                            revision.AttributePanel[j].AssetTypeScreen = screenRevision;
                    }

                    revision.AssetTypeScreen.Add(screenRevision);
                }
            }

            return revision;
        }

        /// <summary>
        /// Returns the AssetType by its name
        /// </summary>
        /// <param name="assetTypeName">Name of assetType to search</param>
        /// <returns>AssetType object if found, null if not</returns>
        public AssetType FindByName(string assetTypeName)
        {
            var data = _unitOfWork.DynEntityConfigRepository.SingleOrDefault(
                d => d.Name == assetTypeName && d.ActiveVersion && d.Active && !d.IsUnpublished);
            return data != null
                ? GetById(data.DynEntityConfigId)
                : null;
        }

        /// <summary>
        /// Returns the existing AssetType attribute by its UID
        /// </summary>
        /// <param name="uid">Unique ID of AssetType attribute</param>
        /// <returns>AssetTypeAttribute object</returns>
        [Obsolete("Use methods of AttributeRepository")]      
        public AssetTypeAttribute GetAttributeByUid(long uid)
        {
            var data = _unitOfWork.DynEntityAttribConfigRepository
                .SingleOrDefault(deac => deac.DynEntityAttribConfigUid == uid);
            return data != null
                ? new AssetTypeAttribute(data, _unitOfWork)
                : null;
        }

        public AssetType GetPredefinedAssetType(PredefinedEntity entity)
        {
            var name = entity.ToString();
            var data = _unitOfWork.PredefinedAttributesRepository
                .SingleOrDefault(pa => pa.Name == name);
            if (data == null)
                throw new Exception(string.Format("Cannot find PredefinedEntity {0}", name));
            return GetById(data.DynEntityConfigID);
        }

        public AssetType GetDraftById(long assetTypeId, int currentRevision)
        {
            // try to find asset type with same ID and revision greater than this one, but not active version
            // and with maximum revision number
            var draft = _unitOfWork.DynEntityConfigRepository
                .Where(dec => dec.DynEntityConfigId == assetTypeId
                              && dec.ActiveVersion == false
                              && dec.Revision > currentRevision)
                .OrderBy(dec => dec.Revision)
                .LastOrDefault();
            return draft != null
                ? GetByUid(draft.DynEntityConfigUid)
                : null;
        }

        /// <summary>
        /// Deletes current AssetType
        /// </summary>
        public void Delete(AssetType assetType)
        {
            assetType.Base.Active = false;
            _unitOfWork.DynEntityConfigRepository.Update(assetType.Base);
            _unitOfWork.Commit();
            Cache<AssetType>.Flush();
        }

        public ValidationResult ValidateNameUniqueness(string assetTypeName, long assetTypeId)
        {
            var existing =
                _unitOfWork.DynEntityConfigRepository.Get(
                    config => config.DBTableName == assetTypeName &&
                              config.DynEntityConfigId != assetTypeId)
                    .Any();

            if (existing)
            {
                return new ValidationResult
                {
                    ResultLines =
                        new List<ValidationResultLine>
                        {
                            new ValidationResultLine(string.Empty)
                            {
                                Message = Resources.AssetTypeAlreadyExistsError
                            }
                        }
                };
            }
            return ValidationResult.Success;
        }

        public bool IsPredefinedAssetType(AssetType typeToCheck, PredefinedEntity entity)
        {
            var expectedType = GetPredefinedAssetType(entity);
            return expectedType.ID == typeToCheck.ID;
        }
    }
}