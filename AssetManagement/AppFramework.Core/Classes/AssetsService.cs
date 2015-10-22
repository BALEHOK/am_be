using System.Transactions;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Calculation;
using AppFramework.Core.Classes.SearchEngine;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.Core.DAL;
using AppFramework.Core.DAL.Adapters;
using AppFramework.Core.Exceptions;
using AppFramework.Core.Interfaces;
using AppFramework.DataProxy;
using AppFramework.Entities;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Caching.Expirations;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using AppFramework.Core.DTO;
using AppFramework.Core.Services;

namespace AppFramework.Core.Classes
{
    public class AssetsService : IAssetsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITableProvider _tableProvider;
        private readonly IAttributeValueFormatter _attributeValueFormatter;
        private readonly IIndexationService _indexationService;
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly DynColumnAdapter _columnAdapter;
        private readonly IAttributeCalculator _attributeCalculator;
        private readonly IDynamicListsService _dynamicListsService;
        private readonly IRightsService _rightsService;

        public AssetsService(
            IUnitOfWork unitOfWork,
            IAssetTypeRepository assetTypeRepository,
            IAttributeValueFormatter attributeValueFormatter,
            IRightsService rightsService)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("IUnitOfWork");
            _unitOfWork = unitOfWork;
            if (assetTypeRepository == null)
                throw new ArgumentNullException("IAssetTypeRepository");
            _assetTypeRepository = assetTypeRepository;
            if (attributeValueFormatter == null)
                throw new ArgumentNullException("IAttributeValueFormatter");
            _attributeValueFormatter = attributeValueFormatter;
            if (rightsService == null)
                throw new ArgumentNullException("rightsService");
            _rightsService = rightsService;

            // TODO : must be passed via DI
            var dtService = new DataTypeService(_unitOfWork);
            _columnAdapter = new DynColumnAdapter(dtService);
            _tableProvider = new DynTableProvider(_unitOfWork, _columnAdapter);
            _indexationService = new IndexationService(_unitOfWork, _assetTypeRepository, this);
            _attributeCalculator = new AttributeCalculator(_unitOfWork, this, _assetTypeRepository);
            _dynamicListsService = new DynamicListsService(_unitOfWork);
        }

        public IEnumerable<Asset> GetAssetsByParameters(string tableName, Dictionary<string, string> parameters)
        {
            var typeData = _unitOfWork.DynEntityConfigRepository.Get(c => c.DBTableName == tableName && c.ActiveVersion).SingleOrDefault();
            if (typeData == null)
                throw new ArgumentException("Invalid table name", "tableName");

            var allTypes = _assetTypeRepository.GetTypeRevisions(typeData.DynEntityConfigId);

            parameters.Add("ActiveVersion", "1");

            var rows = _tableProvider.GetRows(typeData.DynEntityConfigId, parameters);
            var assets = rows.Select(row =>
            {
                var uid = (long) row.Fields.Single(f => f.Name == "DynEntityConfigUid").Value;
                var type = allTypes[uid];
                return new Asset(type, row, _attributeValueFormatter, _assetTypeRepository, this, _unitOfWork, _dynamicListsService);
            }).ToList();

            return assets;
        }

        public IEnumerable<Asset> GetHistoryAssets(long assetTypeId, long assetId)
        {
            var allTypes = _assetTypeRepository.GetTypeRevisions(assetTypeId);

            var options = new Dictionary<string, string>(2)
	        {
	            {AttributeNames.DynEntityId, assetId.ToString()},
	            //{AttributeNames.ActiveVersion, bool.FalseString}
	        };

            var rows = _tableProvider.GetRows(assetTypeId, options, "Revision", true, allTypes);
            var assets = rows.Select(row =>
            {
                var uid = (long)row.Fields.Single(f => f.Name == "DynEntityConfigUid").Value;
                var type = allTypes[uid];
                return new Asset(type, row, _attributeValueFormatter, _assetTypeRepository, this, _unitOfWork, _dynamicListsService);
            }).ToList();

            return assets;
        }

        public IEnumerable<Asset> GetAssetsByParameters(AssetType assetType, Dictionary<string, string> parameters)
        {
            var dataRows = _tableProvider.GetRows(
                assetType.Base,
                parameters);
            var assets = dataRows.Select(row =>
            {
                return new Asset(assetType, row, _attributeValueFormatter, _assetTypeRepository, this, _unitOfWork, _dynamicListsService);
            }).ToList();

            return assets;
        }

        public Asset GetAssetByParameters(AssetType assetType, List<SqlParameter> parameters)
        {
            var dataRow = _tableProvider.GetRows(
                assetType.Base,
                parameters)
                .FirstOrDefault();
            return dataRow != null
                ? new Asset(assetType,
                    dataRow,
                    _attributeValueFormatter,
                    _assetTypeRepository,
                    this,
                    _unitOfWork,
                    _dynamicListsService)
                : null;
        }

        public Asset GetAssetByParameters(AssetType assetType, Dictionary<string, string> parameters)
        {
            var dataRow = _tableProvider.GetRows(
                assetType.Base,
                parameters).FirstOrDefault();
            return dataRow != null
                ? new Asset(
                    assetType,
                    dataRow,
                    _attributeValueFormatter,
                    _assetTypeRepository,
                    this,
                    _unitOfWork,
                    _dynamicListsService)
                : null;
        }

        /// <summary>
        /// Returns first the asset by its type and ID
        /// </summary>
        /// <param name="_assetTypeID">AssetType Id</param>
        /// <returns>Asset</returns>
        public Asset GetFirstActiveAsset(AssetType assetType)
        {
            if (assetType == null)
                throw new ArgumentNullException("AssetType");

            Asset retAsset = null;
            DynRow dataRow = _tableProvider.GetFirstActiveRow(assetType.Base);
            if (dataRow != null)
            {
                retAsset = new Asset(
                    assetType,
                    dataRow,
                    _attributeValueFormatter,
                    _assetTypeRepository,
                    this,
                    _unitOfWork,
                    _dynamicListsService);
            }
            return retAsset;
        }

        public Asset InsertAsset(Asset asset, IDictionary<AssetAttribute, Asset> dependencies)
        {
            // save related assets
            var relatedAssets =
                from dependency in dependencies
                select dependency.Value;
            foreach (var relAsset in relatedAssets)
            {
                InsertAsset(relAsset);
            }

            // update attributes of main asset with ids of related assets
            foreach (var dependency in dependencies)
            {
                var attribute = asset.Attributes.Single(a => a == dependency.Key);
                attribute.ValueAsId = dependency.Value.ID;
            }

            // save main asset
            return InsertAsset(asset);
        }

        public Asset InsertAsset(Asset asset)
        {
            if (asset == null)
                throw new ArgumentNullException("Asset");

            if ((string.IsNullOrEmpty(asset.Name)) &&
                asset.GetConfiguration().AutoGenerateNameType == Enumerators.TypeAutoGenerateName.InsertOnly ||
                asset.GetConfiguration().AutoGenerateNameType == Enumerators.TypeAutoGenerateName.InsertUpdate)
                asset.Name = asset.GenerateName();

            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TransactionManager.MaximumTimeout
            };
            using (var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                long assetInitialUid = 0;
                if (asset.ID == 0)
                {
                    // asset is user
                    if (asset.IsUser)
                    {
                        // user name
                        if (string.IsNullOrWhiteSpace(asset[AttributeNames.Name].Value))
                        {
                            asset.Name = Routines.SanitizeLoginName(
                                String.Format("{0} {1}",
                                    asset.Attributes.Single(a => a.Configuration.Name == AttributeNames.PersonName)
                                        .Value,
                                    asset.Attributes.Single(a => a.Configuration.Name == AttributeNames.PersonSecondName)
                                        .Value));
                        }
                        else
                        {
                            asset.Name = Routines.SanitizeLoginName(asset.Name);
                        }

                        if (Membership.GetUser(asset.Name, false) != null)
                            throw new Exception(string.Format("User with name {0} already exists", asset.Name));

                        // permissions on users
                        if (string.IsNullOrWhiteSpace(asset[AttributeNames.PermissionOnUsers].Value))
                            asset[AttributeNames.PermissionOnUsers].Value = "0";
                    }

                    // this is a new asset with first revision
                    asset[AttributeNames.ActiveVersion].Value = bool.TrueString;
                }
                else
                {
                    // Disable active revisions
                    var assetType = _assetTypeRepository.GetById(asset.GetConfiguration().ID);
                    try
                    {
                        var oldAsset = GetAssetById(asset.ID, assetType);
                        long.TryParse(
                            oldAsset.Attributes.Single(g => g.IsIdentity).Value,
                            out assetInitialUid);

                        _disableActiveRevisions(oldAsset, _unitOfWork);
                        _fixRelations(oldAsset, _unitOfWork);
                    }
                    catch (AssetNotFoundException)
                    {
                        // no revisions to disable
                    }
                }

                _tableProvider.InsertAsset(asset);

                if (asset.ID == 0)
                    asset.ID = asset.UID;

                // create the relations with other assets 
                // for attributes type of MultipleAsset
                FreezeMultipleAssetsRelationship(asset, _unitOfWork, assetInitialUid);

                // save dynlist values
                SaveDynamicLists(asset, _unitOfWork);

                // add to index
                _indexationService.UpdateIndex(asset);

                // get inserted asset from DB          
                asset = GetAssetByUid(asset.UID, asset.GetConfiguration());

                SaveDynamicLists(asset, _unitOfWork);

                // asset is user
                if (asset.IsUser)
                {
                    // Role assigning
                    int roleId = (int)asset[AttributeNames.Role].Data.Value;
                    if (roleId == 0)
                        roleId = (int) PredefinedRoles.OnlyPerson;
                    
                    var closureAsset = asset;
                    _unitOfWork.UserInRoleRepository.Delete(
                        _unitOfWork.UserInRoleRepository.Where(ur => ur.UserId == closureAsset.ID));
                    
                    _unitOfWork.UserInRoleRepository.Insert(new UserInRole {UserId = asset.ID, RoleId = roleId});

                    // Permissons assigning
                    // if action was not invoked from batch, but via HttpContext
                    if (HttpContext.Current != null)
                    {
                        // it's a new administrator, grant all permissions
                        if (assetInitialUid == 0 && 
                            Roles.IsUserInRole(
                                asset.Name, PredefinedRoles.Administrators.ToString()))
                        {
                            var rightsList = new[] {
                                new RightsEntry
                                {
                                    TaxonomyItemId = default(long),
                                    AssetTypeID = default(long),
                                    DepartmentID = default(long),
                                    Permission = Permission.RWRW,
                                    IsDeny = false,
                                    ViewID = default(long)
                                }
                            };
                            _rightsService.SetPermissionsForUser(rightsList, asset.ID, AccessManager.Instance.AuthenticationService.CurrentUserId);
                        }

                        // force rights updating in AC storage
                        AccessManager.Instance.ForceRightsUpdate(asset.ID);
                    }
                }

                // calculate asset after relations are created
                asset = _attributeCalculator.PostCalculateAsset(asset, false);
                // update asset after calculation
                ((DynTableProvider)_tableProvider).UpdateAsset(asset);
                
                _attributeCalculator.CalculateDependencies(asset);

                // commit all
                _unitOfWork.Commit();
                scope.Complete();
            }            

            return asset;
        }

        private static void _fixRelations(Asset asset, IUnitOfWork unitOfWork)
        {
            // Put Ids to Uids to fix the relations with other assets
            if (asset.UID > 0)
            {
                var sb = new StringBuilder();
                sb.AppendFormat("UPDATE [{0}] SET ", asset.GetConfiguration().DBTableName);

                var idsTouids = new List<string>();
                foreach (var attribute in asset.Attributes.Where(a => a.GetConfiguration().IsAsset && a.RelatedAsset != null))
                {
                    idsTouids.Add(String.Format("[{0}]={1}",
                        attribute.GetConfiguration().DBTableFieldName,
                        attribute.RelatedAsset.UID));
                }

                if (idsTouids.Count > 0)
                {
                    sb.Append(String.Join(", ", idsTouids));
                    sb.AppendFormat(" WHERE [{0}]={1}",
                        AttributeNames.DynEntityUid,
                        asset[AttributeNames.DynEntityUid].Value);
                    unitOfWork.SqlProvider.ExecuteNonQuery(sb.ToString(), null);
                }
            }
        }

        private static void _disableActiveRevisions(Asset asset, IUnitOfWork unitOfWork)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("UPDATE [{0}] ", asset.GetConfiguration().DBTableName);
            sb.AppendFormat(" SET [{0}]={1} ", AttributeNames.ActiveVersion, 0);
            sb.AppendFormat(" WHERE [{0}]={1};", AttributeNames.DynEntityId, asset[AttributeNames.DynEntityId].Value);
            unitOfWork.SqlProvider.ExecuteNonQuery(sb.ToString(), null);
        }

        /// <summary>
        /// Creates  relations for all attributes of an asset 
        /// which is type of MultipleAssets 
        /// </summary>
        /// <param name="asset">Asset object</param>
        private void FreezeMultipleAssetsRelationship(Asset asset, IUnitOfWork unitOfWork, long assetInitialUID)
        {
            foreach (var attr in asset.Attributes.Where(a => a.GetConfiguration().IsMultipleAssets))
            {
                long assetTypeAttributeId = attr.GetConfiguration().ID;
                long assetTypeAttributeUid = attr.GetConfiguration().UID;
                var assetType = _assetTypeRepository.GetById(attr.GetConfiguration().RelatedAssetTypeID.Value);

                // put existing entries to history
                var previouslyActiveRecords = _unitOfWork.MultipleAssetsActiveRepository.Get(ma => ma.DynEntityId == asset.ID && ma.DynEntityAttribConfigId == assetTypeAttributeId).ToList();
                foreach (var entity in previouslyActiveRecords)
                {
                    Asset relAsset = GetAssetById(entity.RelatedDynEntityId, assetType);
                    if (relAsset != null)
                    {
                        unitOfWork.MultipleAssetsHistoryRepository.Insert(new MultipleAssetsHistory()
                        {
                            DynEntityUid = assetInitialUID,
                            DynEntityAttribConfigUid = assetTypeAttributeUid,
                            RelatedDynEntityUid = relAsset.UID
                        });
                    }
                }

                // add new active entries
                foreach (KeyValuePair<long, string> idNamePair in attr.MultipleAssets)
                {
                    var relAsset = GetAssetById(idNamePair.Key, assetType);
                    if (relAsset == null)
                        throw new NullReferenceException("Referenced asset cannot be found");
                    var existingEntry = previouslyActiveRecords.SingleOrDefault(ma => ma.RelatedDynEntityId == idNamePair.Key);
                    if (existingEntry == null)
                    {
                        unitOfWork.MultipleAssetsActiveRepository.Insert(new MultipleAssetsActive()
                        {
                            DynEntityId = asset.ID,
                            DynEntityAttribConfigId = assetTypeAttributeId,
                            RelatedDynEntityId = idNamePair.Key
                        });
                    }
                    else
                    {
                        previouslyActiveRecords.Remove(existingEntry);
                    }
                }

                // delete removed entries
                for (int i = previouslyActiveRecords.Count() - 1; i >= 0; i--)
                {
                    unitOfWork.MultipleAssetsActiveRepository.Delete(previouslyActiveRecords[i]);
                }
            }
        }


        /// <summary>
        /// Saves Dynamic lists for particular asset
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="unitOfWork"></param>
        private static void SaveDynamicLists(Asset asset, IUnitOfWork unitOfWork)
        {
            foreach (var attribute in asset.Attributes.Where(attr => attr.IsDynamicList))
            {
                long parent = 0;
                foreach (var dynListValue in attribute.DynamicListValues.OrderBy(d => d.DisplayOrder))
                {
                    var entity = dynListValue.Base;
                    entity.ParentListId = parent;
                    entity.DynEntityAttribConfigUid = attribute.Configuration.UID;
                    entity.DynEntityConfigUid = asset.Configuration.UID;
                    entity.AssetUid = asset.UID;
                    entity.DynListUid = attribute.Configuration.DynamicListUid.Value;
               
                    if (entity.DynListValueUid == 0)
                        unitOfWork.DynListValueRepository.Insert(entity);
                    else
                        unitOfWork.DynListValueRepository.Update(entity);

                    parent = entity.DynListValueUid;
                }
            }
        }

        /// <summary>
        /// Used for move to next location batch job task
        /// </summary>
        /// <param name="assetTypeId"></param>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public IEnumerable<Asset> GetAssetsByAssetTypeIdAndLocation(long assetTypeId, long locationId)
        {
            var result = new List<Asset>();
            var currentAT = _assetTypeRepository.GetById(assetTypeId);
            var hasNextLocation = currentAT.Attributes.Count(a => a.DBTableFieldName == AttributeNames.NextLocationId);

            if (hasNextLocation > 0)
            {
                var options = new Dictionary<string, string>() { { AttributeNames.ActiveVersion, bool.TrueString },
				{AttributeNames.NextLocationId, locationId.ToString() }};
                List<DynRow> rows = _tableProvider.GetRows(
                            currentAT.Base,
                            options,
                            0,
                            long.MaxValue);
                result.AddRange(from row in rows
                                where row != null
                                select new Asset(
                                    currentAT,
                                    row,
                                    _attributeValueFormatter,
                                    _assetTypeRepository,
                                    this,
                                    _unitOfWork,
                                    _dynamicListsService));
            }
            return result;
        }

        /// <summary>
        /// Get items for FAQ section
        /// </summary>
        /// <param name="cultureName">Culture name from Languege table</param>
        /// <returns></returns>
        public IEnumerable<Asset> GetFaqItems(System.Globalization.CultureInfo culture = null, int itemsNumber = 1000)
        {
            var currentAT = _assetTypeRepository.GetPredefinedAssetType(PredefinedEntity.Faq);
            if (!currentAT.Attributes.Any(a => a.DBTableFieldName == "Language"))
                throw new Exception("Language attribute missing for FAQ");

            var options = new Dictionary<string, string>() { { AttributeNames.ActiveVersion, bool.TrueString } };
            var rows = _tableProvider.GetRows(currentAT.Base, options, 0, itemsNumber);
            var faqItems = (from row in rows
                            where row != null
                            select new Asset(
                                    currentAT,
                                    row,
                                    _attributeValueFormatter,
                                    _assetTypeRepository,
                                    this,
                                    _unitOfWork,
                                    _dynamicListsService)).ToList();

            if (culture != null)
                faqItems = faqItems.Where(i => i["Language"].Value == culture.Name).ToList();

            return faqItems;
        }

        /// <summary>
        /// Returns the asset by its type and ID.
        /// </summary>
        /// <param name="assetType">AssetType</param>
        /// <param name="assetId">Asset Id</param>
        /// <returns>Asset</returns>
        public Asset GetAssetById(long assetId, AssetType assetType)
        {
            if (assetType == null)
                throw new ArgumentNullException("assetType");

            var dataRow = _tableProvider.GetRowById(assetType.Base, assetId);
            if (dataRow == null)
                throw new AssetNotFoundException(
                    string.Format("Cannot find asset by given id: {0} (asset type id: {1})",
                        assetId, assetType.ID));

            return new Asset(
                assetType,
                dataRow,
                _attributeValueFormatter,
                _assetTypeRepository,
                this,
                _unitOfWork,
                _dynamicListsService);
        }

        /// <summary>
        /// Returns asset revision
        /// </summary>
        /// <param name="assetType">AssetType</param>
        /// <param name="assetId">Asset Id</param>
        /// <returns>Asset</returns>
        public Asset GetAssetByIdAndRevison(long assetId, AssetType assetType, int revision)
        {
            if (assetType == null)
                throw new ArgumentNullException("assetType");

            var dataRow = _tableProvider.GetRowByIdAndRevision(assetType.Base, assetId, revision);
            if (dataRow == null)
                throw new AssetNotFoundException(
                    string.Format("Cannot find asset by given id ({0}) and revision ({1})  (assettype id: {2})",
                        assetId,
                        revision,
                        assetType.ID));

            return new Asset(
                assetType,
                dataRow,
                _attributeValueFormatter,
                _assetTypeRepository,
                this,
                _unitOfWork,
                _dynamicListsService);
        }

        /// <summary>
        /// Returns the asset by its Uid
        /// </summary>
        /// <param name="assetUid"></param>
        /// <param name="assetTypeUid"></param>
        /// <returns></returns>
        public Asset GetAssetByUid(long assetUid, long assetTypeUid)
        {
            var assetType = _assetTypeRepository.GetByUid(assetTypeUid);
            if (assetType == null)
                throw new AssetTypeNotFoundException();
            return GetAssetByUid(assetUid, assetType);
        }

        /// <summary>
        /// Returns the asset by its Uid
        /// </summary>
        /// <param name="assetUid"></param>
        /// <param name="assetType">Instance of AssetType</param>
        /// <returns>Asset</returns>
        public Asset GetAssetByUid(long assetUid, AssetType assetType)
        {
            if (assetType == null)
                throw new ArgumentNullException("assetType");
            
            DynRow dataRow = _tableProvider.GetRowByUid(assetType.Base, assetUid);

            if (dataRow == null)
                throw new AssetNotFoundException(
                    string.Format("Cannot find asset by given uid: {0} (asset type id: {1})",
                        assetUid, assetType.ID));
            
            var asset = new Asset(assetType, dataRow, _attributeValueFormatter, _assetTypeRepository, this, _unitOfWork, _dynamicListsService);
            return asset;
        }

        /// <summary>
        /// Creates the instance of asset with specific type.        
        /// </summary>
        /// <returns>New asset instance.</returns>
        public Asset CreateAsset(AssetType assetType)
        {
            var dataRow = new DynRow { TableName = assetType.DBTableName };
            dataRow.Fields.AddRange(from attr in assetType.Attributes
                                    select _columnAdapter.ConvertDynEntityAttribConfigToDynColumn(attr.Base));
            dataRow.Fields.Single(f => f.Name == AttributeNames.DynEntityId).Value = 0;
            dataRow.Fields.Single(f => f.Name == AttributeNames.DynEntityUid).Value = 0;
            dataRow.Fields.Single(f => f.Name == AttributeNames.Revision).Value = 1;
            dataRow.Fields.Single(f => f.Name == AttributeNames.DynEntityConfigUid).Value = assetType.UID;
            var newAsset = new Asset(
                assetType,
                dataRow,
                _attributeValueFormatter,
                _assetTypeRepository,
                this,
                _unitOfWork,
                _dynamicListsService);
            return newAsset;
        }

        /// <summary>
        /// Deletes the asset
        /// </summary>
        /// <param name="deletingAsset"></param>
        /// <param name="permission"></param>
        public void DeleteAsset(Asset deletingAsset, Permission permission)
        {
            if (!permission.CanDelete())
                throw new InsufficientPermissionsException();

            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TransactionManager.MaximumTimeout
            };

            var dependenciesFinder = new DependenciesFinder(_unitOfWork, this, _assetTypeRepository);
            var dependencies = dependenciesFinder.GetDependentAssets(deletingAsset);

            using (var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                deletingAsset[AttributeNames.ActiveVersion].Value = false.ToString();
                InsertAsset(deletingAsset);
                _unitOfWork.DeletedEntitiesRepository.Insert(new DeletedEntity()
                {
                    DynEntityId = deletingAsset.ID,
                    DynEntityUid = deletingAsset.UID,
                    DynEntityConfigId = deletingAsset.GetConfiguration().ID
                });

                _unitOfWork.Commit();

                dependencies.ForEach(d =>
                {
                    var calculated = _attributeCalculator.PostCalculateAsset(d);
                    InsertAsset(calculated);
                });

                _unitOfWork.Commit();
                scope.Complete();
            }
        }

        /// <summary>
        /// Returns the list of active assets by given asset type filtered with user's permissions.
        /// </summary>
        /// <param name="assetTypeId">Asset type identifier</param>
        /// <param name="currentUserId">Current User's Id</param>
        /// <param name="rowStart">Start row</param>
        /// <param name="rowsNumber">Rows amount</param>
        /// <returns>Assets collection</returns>  
        public IEnumerable<Asset> GetAssetsByAssetTypeIdAndUser(long assetTypeId, long currentUserId, int? rowStart = null,
            int? rowsNumber = null)
        {
            var assetType = _assetTypeRepository.GetById(assetTypeId);
            return GetAssetsByAssetTypeAndUser(assetType, currentUserId, rowStart, rowsNumber);
        }

        /// <summary>
        /// Returns the list of active assets by given asset type filtered with user's permissions.
        /// </summary>
        /// <param name="assetType">Asset type</param>
        /// <param name="currentUserId">Current User's Id</param>
        /// <param name="rowStart">Start row</param>
        /// <param name="rowsNumber">Rows amount</param>
        /// <returns>Assets collection</returns>        
        public IEnumerable<Asset> GetAssetsByAssetTypeAndUser(AssetType assetType, long currentUserId, int? rowStart = null,
            int? rowsNumber = null)
        {
            var cacheKey = string.Format("_PermittedAssets_{0}_{1}", assetType.ID, currentUserId);
            if (rowStart != null && rowsNumber != null)
                cacheKey = string.Format("_PermittedAssets_{0}_{1}_{2}_{3}", assetType.ID, currentUserId, rowStart,
                    rowsNumber);

            var cacheManager = CacheFactory.GetCacheManager();
            if (cacheManager.GetData(cacheKey) as List<Asset> == null)
            {
                var assetIndexes = _unitOfWork.GetPermittedAssets(assetType.ID, currentUserId, null, null);
                var assets = assetIndexes.Select(i => GetAssetById(i.DynEntityId, assetType));
                if (rowStart.HasValue && rowsNumber.HasValue)
                    assets = assets.Skip(rowStart.Value).Take(rowsNumber.Value);
                cacheManager.Add(cacheKey, assets.ToList(), CacheItemPriority.High, null,
                    new ICacheItemExpiration[] { new AbsoluteTime(TimeSpan.FromSeconds(30)) });
            }
            return cacheManager.GetData(cacheKey) as List<Asset>;
        }

        /// <summary>
        /// Returns the list of active assets by given asset type. No filtering by permissions.
        /// </summary>
        /// <param name="assetType"></param>
        /// <param name="currentUserId"></param>
        /// <param name="rowStart"></param>
        /// <param name="rowsNumber"></param>
        /// <returns></returns>
        [Obsolete("Warning: this method does not applies permissions filtering")]
        public IEnumerable<Asset> GetAssetsByAssetType(AssetType assetType, int? rowStart = null, int? rowsNumber = null)
        {
            var rows = _tableProvider.GetAllRowsByAssetConfiguration(assetType.Base, true, rowStart, rowsNumber);
            return _rowsToAssets(assetType, rows).ToList();
        }

        private IEnumerable<Asset> _rowsToAssets(AssetType at, IEnumerable<DynRow> rows)
        {
            foreach (var row in rows)
            {
                var dynEntityConfigUid =
                    (long)row.Fields.Single(f => f.Name == AttributeNames.DynEntityConfigUid).Value;

                if (at.UID != dynEntityConfigUid)
                {
                    var referencedType = _assetTypeRepository.GetByUid(dynEntityConfigUid);
                    if (referencedType != null)
                        at = referencedType;
                }
                var asset = new Asset(
                    at,
                    row,
                    _attributeValueFormatter,
                    _assetTypeRepository,
                    this,
                    _unitOfWork,
                    _dynamicListsService);
                if (Convert.ToBoolean(asset[AttributeNames.ActiveVersion].Value) == false)
                    continue;
                yield return asset;
            }
        }

        /// <summary>
        /// Returns the list of ID - Name pairs 
        /// </summary>
        /// <returns>IEnumerable object</returns>
        public IEnumerable<KeyValuePair<long, string>> GetIdNameListByAssetType(AssetType assetType)
        {
            if (assetType == null)
                throw new ArgumentNullException();
            var rows = _tableProvider.GetAllRowsByAssetConfiguration(assetType.Base, true);
            foreach (var row in rows)
            {
                if (row != null)
                {
                    yield return
                        new KeyValuePair<long, string>(
                            long.Parse(row.Fields.Find(r => r.Name == AttributeNames.DynEntityId).Value.ToString()),
                            row.Fields.Find(r => r.Name == AttributeNames.Name).Value.ToString());
                }
            }
        }

        /// <summary>
        /// Returns related asset for given attribute
        /// </summary>
        /// <param name="attribute">AssetAttribute</param>
        /// <returns>Asset</returns>
        public Asset GetRelatedAssetByAttribute(AssetAttribute attribute)
        {
            if (!attribute.Configuration.IsAsset 
                || !attribute.ValueAsId.HasValue 
                || attribute.ValueAsId == 0)
                return null;

            var assetType = _assetTypeRepository.GetById(attribute.Configuration.RelatedAssetTypeID.Value);
            try
            {
                if (attribute.ParentAsset != null && attribute.ParentAsset.IsHistory)
                {
                    return GetAssetByUid(attribute.ValueAsId.Value, assetType) ??
                        GetAssetById(attribute.ValueAsId.Value, assetType);
                }
                else
                {
                    return GetAssetById(attribute.ValueAsId.Value, assetType);
                }
            }
            catch (AssetNotFoundException ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Returns collection of related assets as Id-Name pairs for given attribute
        /// </summary>
        /// <param name="attribute">AssetAttribute</param>
        /// <returns>IEnumerable<KeyValuePair<long, string>></returns>
        public IEnumerable<PlainAssetDTO> GetRelatedAssetsByAttribute(AssetAttribute attribute)
        {
            if (!attribute.Configuration.IsMultipleAssets)
                return null;
            bool isActive = true;            
            bool.TryParse(attribute.ParentAsset[AttributeNames.ActiveVersion].Value, out isActive);
            
            if (isActive && attribute.Configuration.IsActiveVersion)
            {
                return GetRelatedActiveAssets(
                        attribute.ParentAsset.ID,
                        attribute.Configuration.ID,
                        attribute.Configuration.RelatedAssetTypeID.Value, 
                        attribute.Configuration.RelatedAssetTypeAttributeID.Value);
            }
            else
            {
                return GetRelatedHistoryAssets(
                        attribute.ParentAsset.UID,
                        attribute.Configuration.UID,
                        attribute.Configuration.RelatedAssetTypeID.Value,
                        attribute.Configuration.RelatedAssetTypeAttributeID.Value);
            }
        }

        public IEnumerable<PlainAssetDTO> GetRelatedActiveAssets(
            long assetId,
            long assetTypeAttributeId,
            long relatedAssetTypeId,
            long relatedAssetTypeAttributeId)
        {
            var relatedAssetsIds = _unitOfWork.MultipleAssetsActiveRepository.Get(ma =>
                ma.DynEntityAttribConfigId == assetTypeAttributeId &&
                ma.DynEntityId == assetId);

            var assetType = _assetTypeRepository.GetById(relatedAssetTypeId);

            foreach (var item in relatedAssetsIds)
            {
                var asset = GetAssetById(item.RelatedDynEntityId, assetType);
                if (asset != null)
                {
                    string displayValue = asset.GetDisplayName(relatedAssetTypeAttributeId);
                    yield return new PlainAssetDTO
                    {
                        Name = displayValue,
                        Id = asset.ID,
                        AssetTypeId = assetType.ID,
                        Revision = asset.Revision
                    };
                }
            }
        }

        private IEnumerable<PlainAssetDTO> GetRelatedHistoryAssets(
            long assetUid,
            long assetTypeAttributeUid,
            long relatedAssetTypeId,
            long relatedAssetTypeAttributeId)
        {
            var relatedAssetsIds = _unitOfWork.MultipleAssetsHistoryRepository.Get(ma =>
                ma.DynEntityAttribConfigUid == assetTypeAttributeUid &&
                ma.DynEntityUid == assetUid);

            var assetType = _assetTypeRepository.GetById(relatedAssetTypeId);

            foreach (var item in relatedAssetsIds)
            {
                var asset = GetAssetByUid(item.RelatedDynEntityUid, assetType);
                if (asset != null)
                {
                    string displayValue = asset.GetDisplayName(relatedAssetTypeAttributeId);
                    yield return new PlainAssetDTO
                    {
                        Name = displayValue,
                        Id = asset.ID,
                        AssetTypeId = assetType.ID,
                        Revision = asset.Revision
                    };
                }
            }
        }

        public void RestoreAsset(Asset asset)
        {
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TransactionManager.MaximumTimeout
            };

            //var dependenciesFinder = new DependenciesFinder(_unitOfWork, this, _assetTypeRepository);
            //var dependencies = dependenciesFinder.GetDependentAssets(asset);

            using (var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                var assetTypeId = asset.GetConfiguration().ID;
                var de = _unitOfWork.DeletedEntitiesRepository.Single(e =>
                    e.DynEntityId == asset.ID &&
                    e.DynEntityUid == asset.UID &&
                    e.DynEntityConfigId == assetTypeId);
                _unitOfWork.DeletedEntitiesRepository.Delete(de);
                _unitOfWork.Commit();

                asset[AttributeNames.ActiveVersion].Value = true.ToString();
                InsertAsset(asset);

                //dependencies.ForEach(d =>
                //{
                //    var calculated = _attributeCalculator.GetCalculatedAsset(d);
                //    InsertAsset(calculated);
                //});
                //_unitOfWork.Commit();

                scope.Complete();
            }
        }

        public Asset CreateAsset(PredefinedEntity predefined)
        {
            var docName = predefined.ToString();
            var entity = _unitOfWork.PredefinedAttributesRepository
                .Single(pa => pa.Name == docName);
            var documentType = _assetTypeRepository
                .GetById(entity.DynEntityConfigID);
            return CreateAsset(documentType);
        }
    }
}
