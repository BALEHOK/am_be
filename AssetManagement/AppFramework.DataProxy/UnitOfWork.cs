using System.Data;

namespace AppFramework.DataProxy
{
    using AppFramework.DataLayer;
    using AppFramework.DataProxy.Providers;
    using AppFramework.DataProxy.Repositories;
    using AppFramework.Entities;
    using Common.Logging;
    using System;
    using System.Collections.Generic;
    using System.Data.EntityClient;
    using System.Data.Objects;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Transactions;
    using System.Web;
    using Enumerations = AppFramework.Entities.Enumerations;

    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        #region FunctionImports

        public ObjectResult<f_cust_GetSqlServerAgentJobs_Result> GetSqlServerAgentJobs()
        {
            return _noCacheContext.GetSqlServerAgentJobs();
        }

        public ObjectResult<f_cust_SearchByKeywords_Result> SearchByKeywords(
            Guid searchId,
            long userId,
            string keywords,
            string configIds = "",
            string taxonomyItemsIds = "",
            bool isActive = true,
            Enumerations.SearchOrder order = Enumerations.SearchOrder.Date,
            int pageNumber = 1,
            int pageSize = 20,
            long? attributeId = null,
            long? assetId = null)
        {
            return _noCacheContext.f_cust_SearchByKeywords(
                searchId,
                userId,
                keywords,
                configIds,
                taxonomyItemsIds,
                isActive,
                (byte)order,
                pageNumber,
                pageSize,
                attributeId,
                assetId);
        }

        public ObjectResult<f_cust_SearchByTypeContext_Result> SearchByTypeContext(
            Guid searchId,
            long userId,
            string configIds = "",
            string taxonomyItemsIds = "",
            bool isActive = true,
            Enumerations.SearchOrder order = Enumerations.SearchOrder.Date,
            int pageNumber = 1,
            int pageSize = 20)
        {
            return _noCacheContext.f_cust_SearchByTypeContext(
                searchId,
                userId,
                configIds,
                taxonomyItemsIds,
                isActive,
                (byte)order,
                pageNumber,
                pageSize);
        }

        public ObjectResult<f_cust_GetSrchCount_Result> GetSearchResultCounters(
            Guid searchId,
            long userId,
            string keywords = "",
            string configIds = "",
            string taxonomyItemsIds = "",
            bool isActive = true,
            bool isTypeContextSearch = false)
        {
            return _noCacheContext.f_cust_GetSrchCount(searchId, userId, keywords, configIds, taxonomyItemsIds, isActive, isTypeContextSearch);
        }

        #endregion

        public void RebuildTriggers(long dynEntityConfigUid)
        {
            _noCacheContext.f_cust_RebuildTriggers(dynEntityConfigUid);
        }

        public void RestoreDeletedItem(long dynEntityId, long dynEntityConfigId)
        {
            _noCacheContext.RestoreDeletedItem(dynEntityId, dynEntityConfigId);
        }

        public ObjectResult<DynEntityIndex> GetPermittedAssets(long assetTypeId, long userId, int? rowStart,
            int? rowsNumber)
        {
            return _noCacheContext.GetPermittedAssets(assetTypeId, userId, rowStart, rowsNumber);
        }

        public bool GetPermittedTask(long assetTypeId, long userId, long taxonomyItemId)
        {
            return _noCacheContext.GetPermittedTask(assetTypeId, userId, taxonomyItemId).FirstOrDefault().Value;
        }

        public ObjectResult<int?> GetPermittedAssetsCount(long assetTypeId, long userId)
        {
            return _noCacheContext.GetPermittedAssetsCount(assetTypeId, userId);
        }

        public ObjectResult<AppFramework.Entities.StockLocationInfo> GetStocksByLocation(long assetId, long assetTypeId)
        {
            return _noCacheContext.GetStocksByLocation(assetId, assetTypeId);
        }

        public IEnumerable<f_cust_GetChildAssets_Result> GetChildAssets(long assetTypeId)
        {
            return _noCacheContext.f_cust_GetChildAssets(assetTypeId);
        }

        public bool IsValueUnique(string dynEntityTableName, string columnName, string value, Nullable<long> excludeDynEntityId)
        {
            var outputParameter = new ObjectParameter("result", typeof(bool));
            _noCacheContext.IsValueUnique(dynEntityTableName, columnName, value, excludeDynEntityId, outputParameter);
            return (bool)outputParameter.Value;
        }
        
        public IEnumerable<f_cust_GetReports_Result> GetReports()
        {
            return _noCacheContext.f_cust_GetReports();
        } 

        public IEnumerable<ActiveTask> GetTasks(long userId)
        {
            return _noCacheContext.f_cust_GetTasks(userId);
        }

        public IEnumerable<long> GetPermittedTasks(long userId)
        {
            return _noCacheContext.ExecuteStoreQuery<long>("SELECT * FROM [dbo].[f_GetGrantedTaskIds](@UserId)", new SqlParameter("@UserId", SqlDbType.BigInt) {Value = userId});
        } 

        #region IDataRepository: Repositories

        public IDataRepository<DeletedEntity> DeletedEntitiesRepository
        {
            get
            {
                if (_repositoryDeletedEntities == null)
                {
                    _repositoryDeletedEntities = new DataRepository<DeletedEntity>(_context);
                }
                return _repositoryDeletedEntities;
            }
        }
        private DataRepository<DeletedEntity> _repositoryDeletedEntities;

        public IDataRepository<DynEntityAttribScreens> DynEntityAttribScreensRepository
        {
            get
            {
                if (_repositoryDynEntityAttribScreens == null)
                {
                    _repositoryDynEntityAttribScreens = new DataRepository<DynEntityAttribScreens>(_context);
                }
                return _repositoryDynEntityAttribScreens;
            }
        }
        private IDataRepository<DynEntityAttribScreens> _repositoryDynEntityAttribScreens;

        public IDataRepository<Languages> LanguagesRepository
        {
            get
            {
                if (_repositoryLanguages == null)
                {
                    _repositoryLanguages = new DataRepository<Languages>(_context);
                }
                return _repositoryLanguages;
            }
        }
        private IDataRepository<Languages> _repositoryLanguages;

        public IDataRepository<StringResources> StringResourcesRepository
        {
            get
            {
                if (_repositoryStringResources == null)
                {
                    _repositoryStringResources = new DataRepository<StringResources>(_context);
                }
                return _repositoryStringResources;
            }
        }
        private IDataRepository<StringResources> _repositoryStringResources;

        public IDataRepository<AssetsTaxonomies> AssetsTaxonomiesRepository
        {
            get
            {
                if (_repositoryAssetsTaxonomies == null)
                {
                    _repositoryAssetsTaxonomies = new DataRepository<AssetsTaxonomies>(_context);
                }
                return _repositoryAssetsTaxonomies;
            }
        }
        private IDataRepository<AssetsTaxonomies> _repositoryAssetsTaxonomies;

        public ITaxonomyItemRepository TaxonomyItemRepository
        {
            get
            {
                if (_repositoryTaxonomyItem == null)
                {
                    _repositoryTaxonomyItem = new TaxonomyItemRepository(_context);
                }
                return _repositoryTaxonomyItem;
            }
        }
        private ITaxonomyItemRepository _repositoryTaxonomyItem;

        public TaxonomyRepository TaxonomyRepository
        {
            get
            {
                if (_repositoryTaxonomy == null)
                {
                    _repositoryTaxonomy = new TaxonomyRepository(_context);
                }
                return _repositoryTaxonomy;
            }
        }
        private TaxonomyRepository _repositoryTaxonomy;

        public IDataRepository<DynEntityConfigTaxonomy> DynEntityConfigTaxonomyRepository
        {
            get
            {
                if (_repositoryDynEntityConfigTaxonomy == null)
                {
                    _repositoryDynEntityConfigTaxonomy = new DataRepository<DynEntityConfigTaxonomy>(_context);
                }
                return _repositoryDynEntityConfigTaxonomy;
            }
        }
        private IDataRepository<DynEntityConfigTaxonomy> _repositoryDynEntityConfigTaxonomy;

        public IDataRepository<DynEntityTaxonomyItem> DynEntityTaxonomyItemRepository
        {
            get
            {
                if (_repositoryDynEntityTaxonomyItem == null)
                {
                    _repositoryDynEntityTaxonomyItem = new DataRepository<DynEntityTaxonomyItem>(_context);
                }
                return _repositoryDynEntityTaxonomyItem;
            }
        }
        private IDataRepository<DynEntityTaxonomyItem> _repositoryDynEntityTaxonomyItem;

        public IDataRepository<DynEntityTaxonomyItemHistory> DynEntityTaxonomyItemHistoryRepository
        {
            get
            {
                if (_repositoryDynEntityTaxonomyItemHistory == null)
                {
                    _repositoryDynEntityTaxonomyItemHistory = new DataRepository<DynEntityTaxonomyItemHistory>(_context);
                }
                return _repositoryDynEntityTaxonomyItemHistory;
            }
        }
        private IDataRepository<DynEntityTaxonomyItemHistory> _repositoryDynEntityTaxonomyItemHistory;

        public IDataRepository<Rights> RightsRepository
        {
            get
            {
                if (_repositoryRights == null)
                {
                    _repositoryRights = new DataRepository<Rights>(_context);
                }
                return _repositoryRights;
            }
        }
        private IDataRepository<Rights> _repositoryRights;

        public IDataRepository<Reservation> ReservationRepository
        {
            get
            {
                if (_repositoryReservation == null)
                {
                    _repositoryReservation = new DataRepository<Reservation>(_context);
                }
                return _repositoryReservation;
            }
        }
        private IDataRepository<Reservation> _repositoryReservation;

        public IDataRepository<PredefinedAttributes> PredefinedAttributesRepository
        {
            get
            {
                if (_repositoryPredefinedAttributes == null)
                {
                    _repositoryPredefinedAttributes = new DataRepository<PredefinedAttributes>(_context);
                }
                return _repositoryPredefinedAttributes;
            }
        }
        private IDataRepository<PredefinedAttributes> _repositoryPredefinedAttributes;

        public IDataRepository<PredefinedAsset> PredefinedAssetRepository
        {
            get
            {
                if (_repositoryPredefinedAsset == null)
                {
                    _repositoryPredefinedAsset = new DataRepository<PredefinedAsset>(_context);
                }
                return _repositoryPredefinedAsset;
            }
        }
        private IDataRepository<PredefinedAsset> _repositoryPredefinedAsset;

        public IDataRepository<ZipCode> ZipCodeRepository
        {
            get
            {
                if (_repositoryZipCode == null)
                {
                    _repositoryZipCode = new DataRepository<ZipCode>(_context);
                }
                return _repositoryZipCode;
            }
        }
        private IDataRepository<ZipCode> _repositoryZipCode;

        public IDataRepository<Place> PlaceRepository
        {
            get
            {
                if (_repositoryPlace == null)
                {
                    _repositoryPlace = new DataRepository<Place>(_context);
                }
                return _repositoryPlace;
            }
        }
        private IDataRepository<Place> _repositoryPlace;

        public IDataRepository<AttributePanel> AttributePanelRepository
        {
            get
            {
                if (_repositoryAttributePanel == null)
                {
                    _repositoryAttributePanel = new DataRepository<AttributePanel>(_context);
                }
                return _repositoryAttributePanel;
            }
        }
        private IDataRepository<AttributePanel> _repositoryAttributePanel;

        public IDataRepository<AttributePanelAttribute> AttributePanelAttributeRepository
        {
            get
            {
                if (_repositoryAttributePanelAttribute == null)
                {
                    _repositoryAttributePanelAttribute = new DataRepository<AttributePanelAttribute>(_context);
                }
                return _repositoryAttributePanelAttribute;
            }
        }
        private IDataRepository<AttributePanelAttribute> _repositoryAttributePanelAttribute;


        public IDataRepository<MeasureUnit> MeasureUnitRepository
        {
            get
            {
                if (_repositoryMeasureUnit == null)
                {
                    _repositoryMeasureUnit = new DataRepository<MeasureUnit>(_context);
                }
                return _repositoryMeasureUnit;
            }
        }
        private IDataRepository<MeasureUnit> _repositoryMeasureUnit;

        public IDataRepository<ScreenLayout> ScreenLayoutRepository
        {
            get
            {
                if (_repositoryScreenLayout == null)
                {
                    _repositoryScreenLayout = new DataRepository<ScreenLayout>(_context);
                }
                return _repositoryScreenLayout;
            }
        }
        private IDataRepository<ScreenLayout> _repositoryScreenLayout;

        public IDataRepository<DynEntityType> DynEntityTypeRepository
        {
            get
            {
                if (_repositoryDynEntityType == null)
                {
                    _repositoryDynEntityType = new DataRepository<DynEntityType>(_context);
                }
                return _repositoryDynEntityType;
            }
        }
        private IDataRepository<DynEntityType> _repositoryDynEntityType;

        public IDataRepository<SearchOperators> SearchOperatorsRepository
        {
            get
            {
                if (_repositorySearchOperators == null)
                {
                    _repositorySearchOperators = new DataRepository<SearchOperators>(_context);
                }
                return _repositorySearchOperators;
            }
        }
        private IDataRepository<SearchOperators> _repositorySearchOperators;

        public IDataRepository<Context> ContextRepository
        {
            get
            {
                if (_repositoryContext == null)
                {
                    _repositoryContext = new DataRepository<Context>(_context);
                }
                return _repositoryContext;
            }
        }
        private IDataRepository<Context> _repositoryContext;

        public IDataRepository<MultipleAssetsHistory> MultipleAssetsHistoryRepository
        {
            get
            {
                if (_repositoryMultipleAssetsHistory == null)
                {
                    _repositoryMultipleAssetsHistory = new DataRepository<MultipleAssetsHistory>(_context);
                }
                return _repositoryMultipleAssetsHistory;
            }
        }
        private IDataRepository<MultipleAssetsHistory> _repositoryMultipleAssetsHistory;

        public IDataRepository<MultipleAssetsActive> MultipleAssetsActiveRepository
        {
            get
            {
                if (_repositoryMultipleAssetsActive == null)
                {
                    _repositoryMultipleAssetsActive = new DataRepository<MultipleAssetsActive>(_context);
                }
                return _repositoryMultipleAssetsActive;
            }
        }
        private IDataRepository<MultipleAssetsActive> _repositoryMultipleAssetsActive;

        public IDataRepository<AppSettings> AppSettingsRepository
        {
            get
            {
                if (_repositoryAppSettings == null)
                {
                    _repositoryAppSettings = new DataRepository<AppSettings>(_context);
                }
                return _repositoryAppSettings;
            }
        }
        private IDataRepository<AppSettings> _repositoryAppSettings;

        public IDataRepository<ValidationList> ValidationListRepository
        {
            get
            {
                if (_repositoryValidationList == null)
                {
                    _repositoryValidationList = new DataRepository<ValidationList>(_context);
                }
                return _repositoryValidationList;
            }
        }
        private IDataRepository<ValidationList> _repositoryValidationList;

        public IDataRepository<DynEntityAttribValidation> DynEntityAttribValidationRepository
        {
            get
            {
                if (_repositoryDynEntityAttribValidation == null)
                {
                    _repositoryDynEntityAttribValidation = new DataRepository<DynEntityAttribValidation>(_context);
                }
                return _repositoryDynEntityAttribValidation;
            }
        }
        private IDataRepository<DynEntityAttribValidation> _repositoryDynEntityAttribValidation;

        public IDataRepository<ValidationOperand> ValidationOperandRepository
        {
            get
            {
                if (_repositoryValidationOperand == null)
                {
                    _repositoryValidationOperand = new DataRepository<ValidationOperand>(_context);
                }
                return _repositoryValidationOperand;
            }
        }
        private IDataRepository<ValidationOperand> _repositoryValidationOperand;

        public IDataRepository<ValidationOperator> ValidationOperatorRepository
        {
            get
            {
                if (_repositoryValidationOperator == null)
                {
                    _repositoryValidationOperator = new DataRepository<ValidationOperator>(_context);
                }
                return _repositoryValidationOperator;
            }
        }
        private IDataRepository<ValidationOperator> _repositoryValidationOperator;

        public IDataRepository<ValidationOperandValue> ValidationOperandValueRepository
        {
            get
            {
                if (_repositoryValidationOperandValue == null)
                {
                    _repositoryValidationOperandValue = new DataRepository<ValidationOperandValue>(_context);
                }
                return _repositoryValidationOperandValue;
            }
        }
        private IDataRepository<ValidationOperandValue> _repositoryValidationOperandValue;


        public IDataRepository<DynEntityTransaction> DynEntityTransactionRepository
        {
            get
            {
                if (_repositoryDynEntityTransaction == null)
                {
                    _repositoryDynEntityTransaction = new DataRepository<DynEntityTransaction>(_context);
                }
                return _repositoryDynEntityTransaction;
            }
        }
        private IDataRepository<DynEntityTransaction> _repositoryDynEntityTransaction;

        public IDataRepository<DynEntityConfig> DynEntityConfigRepository
        {
            get
            {
                if (_dynEntityConfigRepository == null)
                {
                    _dynEntityConfigRepository = new DataRepository<DynEntityConfig>(_context);
                }
                return _dynEntityConfigRepository;
            }
        }
        private IDataRepository<DynEntityConfig> _dynEntityConfigRepository;

        public IDataRepository<DynEntityAttribConfig> DynEntityAttribConfigRepository
        {
            get
            {
                if (_repositoryDynEntityAttribConfig == null)
                {
                    _repositoryDynEntityAttribConfig = new DataRepository<DynEntityAttribConfig>(_context);
                }
                return _repositoryDynEntityAttribConfig;
            }
        }
        private IDataRepository<DynEntityAttribConfig> _repositoryDynEntityAttribConfig;

        public IDataRepository<UserInRole> UserInRoleRepository
        {
            get
            {
                if (_repositoryUserInRole == null)
                {
                    _repositoryUserInRole = new DataRepository<UserInRole>(_context);
                }
                return _repositoryUserInRole;
            }
        }
        private IDataRepository<UserInRole> _repositoryUserInRole;

        public IDataRepository<ImportExport> ImportExportRepository
        {
            get
            {
                if (_repositoryImportExport == null)
                {
                    _repositoryImportExport = new DataRepository<ImportExport>(_context);
                }
                return _repositoryImportExport;
            }
        }
        private IDataRepository<ImportExport> _repositoryImportExport;

        public IDataRepository<IndexActiveDynEntities> IndexActiveDynEntitiesRepository
        {
            get
            {
                if (_repositoryIndexActiveDynEntities == null)
                {
                    _repositoryIndexActiveDynEntities = new DataRepository<IndexActiveDynEntities>(_context);
                }
                return _repositoryIndexActiveDynEntities;
            }
        }
        private IDataRepository<IndexActiveDynEntities> _repositoryIndexActiveDynEntities;

        public IDataRepository<IndexHistoryDynEntities> IndexHistoryDynEntitiesRepository
        {
            get
            {
                if (_repositoryIndexHistoryDynEntities == null)
                {
                    _repositoryIndexHistoryDynEntities = new DataRepository<IndexHistoryDynEntities>(_context);
                }
                return _repositoryIndexHistoryDynEntities;
            }
        }
        private IDataRepository<IndexHistoryDynEntities> _repositoryIndexHistoryDynEntities;

        public IDataRepository<DynEntityIndex> DynEntityIndexRepository
        {
            get
            {
                if (_repositoryDynEntityIndex == null)
                {
                    _repositoryDynEntityIndex = new DataRepository<DynEntityIndex>(_context);
                }
                return _repositoryDynEntityIndex;
            }
        }
        private IDataRepository<DynEntityIndex> _repositoryDynEntityIndex;

        public IDataRepository<BatchJob> BatchJobRepository
        {
            get
            {
                if (_repositoryBatchJob == null)
                {
                    _repositoryBatchJob = new DataRepository<BatchJob>(_context);
                }
                return _repositoryBatchJob;
            }
        }
        private IDataRepository<BatchJob> _repositoryBatchJob;

        public IDataRepository<BatchAction> BatchActionRepository
        {
            get
            {
                if (_repositoryBatchAction == null)
                {
                    _repositoryBatchAction = new DataRepository<BatchAction>(_context);
                }
                return _repositoryBatchAction;
            }
        }
        private IDataRepository<BatchAction> _repositoryBatchAction;


        public IDataRepository<BatchSchedule> BatchScheduleRepository
        {
            get
            {
                if (_repositoryBatchSchedule == null)
                {
                    _repositoryBatchSchedule = new DataRepository<BatchSchedule>(_context);
                }
                return _repositoryBatchSchedule;
            }
        }
        private IDataRepository<BatchSchedule> _repositoryBatchSchedule;

        public IDataRepository<DynList> DynListRepository
        {
            get
            {
                if (_repositoryDynList == null)
                {
                    _repositoryDynList = new DataRepository<DynList>(_context);
                }
                return _repositoryDynList;
            }
        }
        private IDataRepository<DynList> _repositoryDynList;

        public IDataRepository<DynListItem> DynListItemRepository
        {
            get
            {
                if (_repositoryDynListItem == null)
                {
                    _repositoryDynListItem = new DataRepository<DynListItem>(_context);
                }
                return _repositoryDynListItem;
            }
        }
        private IDataRepository<DynListItem> _repositoryDynListItem;

        public IDataRepository<DynListValue> DynListValueRepository
        {
            get
            {
                if (_repositoryDynListValue == null)
                {
                    _repositoryDynListValue = new DataRepository<DynListValue>(_context);
                }
                return _repositoryDynListValue;
            }
        }
        private IDataRepository<DynListValue> _repositoryDynListValue;

        public IDataRepository<Report> ReportRepository
        {
            get
            {
                if (_repositoryReport == null)
                {
                    _repositoryReport = new DataRepository<Report>(_context);
                }
                return _repositoryReport;
            }
        }
        private IDataRepository<Report> _repositoryReport;

        public IDataRepository<DataType> DataTypeRepository
        {
            get
            {
                if (_repositoryDataType == null)
                {
                    _repositoryDataType = new DataRepository<DataType>(_context);
                }
                return _repositoryDataType;
            }
        }
        private IDataRepository<DataType> _repositoryDataType;

        public IDataRepository<SearchTracking> SearchTrackingRepository
        {
            get
            {
                if (_repositorySearchTracking == null)
                {
                    _repositorySearchTracking = new DataRepository<SearchTracking>(_context);
                }
                return _repositorySearchTracking;
            }
        }
        private IDataRepository<SearchTracking> _repositorySearchTracking;

        public IDataRepository<Task> TaskRepository
        {
            get
            {
                if (_repositoryTask == null)
                {
                    _repositoryTask = new DataRepository<Task>(_context);
                }
                return _repositoryTask;
            }
        }
        private IDataRepository<Task> _repositoryTask;


        public IDataRepository<TaskRights> TaskRightRepository
        {
            get
            {
                if (_repositoryTaskRight == null)
                {
                    _repositoryTaskRight = new DataRepository<TaskRights>(_context);
                }
                return _repositoryTaskRight;
            }
        }
        private IDataRepository<TaskRights> _repositoryTaskRight;

        private IDataRepository<AssetTypeScreen> _respositoryAssetTypeScreen;
        public IDataRepository<AssetTypeScreen> AssetTypeScreenRepository
        {
            get
            {
                if (_respositoryAssetTypeScreen == null)
                {
                    _respositoryAssetTypeScreen = new DataRepository<AssetTypeScreen>(_context);
                }
                return _respositoryAssetTypeScreen;
            }
        }

        private IDataRepository<DynEntityUser> _respositoryUsers;
        public IDataRepository<DynEntityUser> DynEntityUserRepository
        {
            get
            {
                if (_respositoryUsers == null)
                {
                    _respositoryUsers = new DataRepository<DynEntityUser>(_context);
                }
                return _respositoryUsers;
            }
        }

        private IDataRepository<DynEntityContextAttributesValues> _respositoryDynEntityContextAttributesValues;
        public IDataRepository<DynEntityContextAttributesValues> DynEntityContextAttributesValuesRepository
        {
            get
            {
                if (_respositoryDynEntityContextAttributesValues == null)
                {
                    _respositoryDynEntityContextAttributesValues = new DataRepository<DynEntityContextAttributesValues>(_context);
                }
                return _respositoryDynEntityContextAttributesValues;
            }
        }

        private IDataRepository<SearchQuery> _searchQueryRepository;
        public IDataRepository<SearchQuery> SearchQueryRepository
        {
            get
            {
                if (_searchQueryRepository == null)
                {
                    _searchQueryRepository = new DataRepository<SearchQuery>(_context);
                }
                return _searchQueryRepository;
            }
        }

        #endregion

        #region IDataProvider: Base providers for plain SQL querying
        /// <summary>
        /// Gets the Entity Framework data provider (requires EntitySQL syntax)
        /// </summary>
        public IDataProvider EntityProvider
        {
            get
            {
                if (_entityProvider == null)
                {
                    DataEntities context = _context;
                    if (_context.GetType() != typeof(DataEntities))
                    {
                        context = new DataEntities();
                    }
                    _entityProvider = new Providers.EntityDataProvider(context);
                }
                return _entityProvider;
            }
        }
        private IDataProvider _entityProvider;

        /// <summary>
        /// Gets the Sql data provider (requires T-SQL syntax)
        /// </summary>
        public IDataProvider SqlProvider
        {
            get
            {
                if (_sqlProvider == null)
                {
                    EntityConnection cnn;
                    if (_context.GetType() == typeof (DataEntities))
                    {
                        cnn = (EntityConnection) _context.Connection;
                    }
                    else
                    {
                        var plaincontext = new DataEntities();
                        cnn = (EntityConnection) plaincontext.Connection;
                    }
                    _sqlProvider = new Providers.SqlDataProvider((SqlConnection) cnn.StoreConnection);
                }
                return _sqlProvider;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                _sqlProvider = value;
            }
        }

        private IDataProvider _sqlProvider;
        #endregion

        /// <summary>
        /// Object context with Caching and Logging
        /// </summary>
        private readonly DataEntities _context;
        private readonly DataEntities _noCacheContext;

        public UnitOfWork()
            : this(new EntityConnection("name=DataEntities"))
        {
        }

        public UnitOfWork(EntityConnection connection)
        {
            _context = HttpContext.Current != null
               ? new ExtendedDataEntities(connection)
               : new DataEntities(connection);
            _noCacheContext = new DataEntities(connection);

            //_logger.DebugFormat("instanciate UnitOfWork #{0}", GetHashCode());
        }

        public void Dispose()
        {
            if (_context != null)
                _context.Dispose();
            if (_noCacheContext != null)
                _noCacheContext.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Commit()
        {            
            _context.SaveChanges();
        }
    }
}
