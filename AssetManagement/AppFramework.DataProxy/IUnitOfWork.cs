using AppFramework.DataProxy.Providers;
using AppFramework.DataProxy.Repositories;
using AppFramework.Entities;
using System;
using System.Collections.Generic;
using System.Data.Objects;

namespace AppFramework.DataProxy
{
    public interface IUnitOfWork
    {
        ObjectResult<f_cust_GetSqlServerAgentJobs_Result> GetSqlServerAgentJobs();

        ObjectResult<f_cust_SearchByKeywords_Result> SearchByKeywords(
            Guid searchId,
            long userId,
            string keywords,
            string configIds = "",
            string taxonomyItemsIds = "",
            bool isActive = true,
            Enumerations.SearchOrder order = Enumerations.SearchOrder.Date,
            int pageNumber = 1,
            int pageSize = 20);

        ObjectResult<f_cust_SearchByTypeContext_Result> SearchByTypeContext(
            Guid searchId,
            long userId,
            string configIds = "",
            string taxonomyItemsIds = "",
            bool isActive = true,
            Enumerations.SearchOrder order = Enumerations.SearchOrder.Date,
            int pageNumber = 1,
            int pageSize = 20);

        ObjectResult<f_cust_GetSrchCount_Result> GetSearchResultCounters(
            Guid searchId,
            long userId,
            string keywords = "",
            string configIds = "",
            string taxonomyItemsIds = "",
            bool isActive = true,
            bool isTypeContextSearch = false);

        void RebuildTriggers(long dynEntityConfigUid);
        void RestoreDeletedItem(long dynEntityId, long dynEntityConfigId);

        ObjectResult<DynEntityIndex> GetPermittedAssets(long assetTypeId, long userId, int? rowStart,
            int? rowsNumber);

        bool GetPermittedTask(long assetTypeId, long userId, long taxonomyItemId);
        ObjectResult<int?> GetPermittedAssetsCount(long assetTypeId, long userId);
        ObjectResult<StockLocationInfo> GetStocksByLocation(long assetId, long assetTypeId);
        IEnumerable<f_cust_GetChildAssets_Result> GetChildAssets(long assetTypeId);
        bool IsValueUnique(string dynEntityTableName, string columnName, string value, Nullable<long> excludeDynEntityId);
        int GetMaxSearchId();
        ITaxonomyItemRepository TaxonomyItemRepository { get; }
        TaxonomyRepository TaxonomyRepository { get; }

        IDataRepository<Context> ContextRepository { get; }
        IDataRepository<DeletedEntity> DeletedEntitiesRepository { get; }
        IDataRepository<DynEntityAttribScreens> DynEntityAttribScreensRepository { get; }
        IDataRepository<Languages> LanguagesRepository { get; }
        IDataRepository<StringResources> StringResourcesRepository { get; }
        IDataRepository<AssetsTaxonomies> AssetsTaxonomiesRepository { get; }
        IDataRepository<DynEntityConfigTaxonomy> DynEntityConfigTaxonomyRepository { get; }
        IDataRepository<Rights> RightsRepository { get; }
        IDataRepository<Reservation> ReservationRepository { get; }
        IDataRepository<PredefinedAttributes> PredefinedAttributesRepository { get; }
        IDataRepository<PredefinedAsset> PredefinedAssetRepository { get; }
        IDataRepository<ZipCode> ZipCodeRepository { get; }
        IDataRepository<Place> PlaceRepository { get; }
        IDataRepository<AttributePanel> AttributePanelRepository { get; }
        IDataRepository<AttributePanelAttribute> AttributePanelAttributeRepository { get; }
        IDataRepository<MeasureUnit> MeasureUnitRepository { get; }
        IDataRepository<ScreenLayout> ScreenLayoutRepository { get; }
        IDataRepository<DynEntityType> DynEntityTypeRepository { get; }
        IDataRepository<SearchOperators> SearchOperatorsRepository { get; }
        IDataRepository<MultipleAssetsHistory> MultipleAssetsHistoryRepository { get; }
        IDataRepository<MultipleAssetsActive> MultipleAssetsActiveRepository { get; }
        IDataRepository<AppSettings> AppSettingsRepository { get; }
        IDataRepository<ValidationList> ValidationListRepository { get; }
        IDataRepository<DynEntityAttribValidation> DynEntityAttribValidationRepository { get; }
        IDataRepository<ValidationOperand> ValidationOperandRepository { get; }
        IDataRepository<ValidationOperator> ValidationOperatorRepository { get; }
        IDataRepository<ValidationOperandValue> ValidationOperandValueRepository { get; }
        IDataRepository<DynEntityTransaction> DynEntityTransactionRepository { get; }
        IDataRepository<DynEntityConfig> DynEntityConfigRepository { get; }
        IDataRepository<DynEntityAttribConfig> DynEntityAttribConfigRepository { get; }
        IDataRepository<UserInRole> UserInRoleRepository { get; }
        IDataRepository<ImportExport> ImportExportRepository { get; }
        IDataRepository<IndexActiveDynEntities> IndexActiveDynEntitiesRepository { get; }
        IDataRepository<IndexHistoryDynEntities> IndexHistoryDynEntitiesRepository { get; }
        IDataRepository<DynEntityIndex> DynEntityIndexRepository { get; }
        IDataRepository<BatchJob> BatchJobRepository { get; }
        IDataRepository<BatchAction> BatchActionRepository { get; }
        IDataRepository<BatchSchedule> BatchScheduleRepository { get; }
        IDataRepository<DynList> DynListRepository { get; }
        IDataRepository<DynListItem> DynListItemRepository { get; }
        IDataRepository<DynListValue> DynListValueRepository { get; }
        IDataRepository<Report> ReportRepository { get; }
        IDataRepository<ReportLayout> ReportLayoutRepository { get; }
        IDataRepository<DataType> DataTypeRepository { get; }
        IDataRepository<SearchTracking> SearchTrackingRepository { get; }
        IDataRepository<Task> TaskRepository { get; }
        IDataRepository<TaskRights> TaskRightRepository { get; }
        IDataRepository<AssetTypeScreen> AssetTypeScreenRepository { get; }
        IDataRepository<DynEntityUser> DynEntityUserRepository { get; }
        IDataRepository<DynEntityContextAttributesValues> DynEntityContextAttributesValuesRepository { get; }

        /// <summary>
        /// Gets the Entity Framework data provider (requires EntitySQL syntax)
        /// </summary>
        IDataProvider EntityProvider { get; }

        /// <summary>
        /// Gets the Sql data provider (requires T-SQL syntax)
        /// </summary>
        IDataProvider SqlProvider { get; set; }

        void Dispose();
        void Commit();
        IEnumerable<f_cust_GetReports_Result> GetReports();
        IEnumerable<ActiveTask> GetTasks();
    }
}