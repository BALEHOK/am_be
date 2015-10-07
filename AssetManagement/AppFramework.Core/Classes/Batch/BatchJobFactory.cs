using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes.IE;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;
using AppFramework.DataProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace AppFramework.Core.Classes.Batch
{
    public interface IBatchJobFactory
    {
        BatchJob CreateRebuildIndexJob(long currentUserId, bool rebuildHistoryIndex = false);

        BatchJob CreateSyncAssetsJob(SyncAssetsParameters parameters);

        BatchJob CreateImportAssetsJob(long currentUserId, string filePath, long assetTypeId, BindingInfo bindings, List<string> sheets, bool deleteSourceOnSuccess);

        BatchJob CreateTaxonomyJob(long _taxonomyUid, long currentUserId);

        BatchJob CreatePublishTypeJob(long atId, long currentUserId, bool newRevision = false);

        BatchJob CreateLocationMoveJob(long currentUserId, BatchActionType type, object[] args = null);

        BatchJob CreateUpdateAssetsJob(string searchId, string typeUid, long userId, List<AttributeElement> attributeElements);
    }

    public class BatchJobFactory : IBatchJobFactory
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBatchJobManager _batchJobManager;
        private readonly IBatchActionFactory _batchActionFactory;
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly ITaxonomyService _taxonomyService;

        public BatchJobFactory(
            IUnitOfWork unitOfWork,
            IAssetTypeRepository assetTypeRepository,
            IBatchJobManager batchJobManager,
            IBatchActionFactory batchActionFactory,
            ITaxonomyService taxonomyService)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("IUnitOfWork");
            if (batchJobManager == null)
                throw new ArgumentNullException("IBatchJobManager");
            if (batchActionFactory == null)
                throw new ArgumentNullException("batchActionFactory");
            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            if (taxonomyService == null)
                throw new ArgumentNullException("taxonomyService");
            _assetTypeRepository = assetTypeRepository;
            _batchActionFactory = batchActionFactory;
            _unitOfWork = unitOfWork;
            _batchJobManager = batchJobManager;
            _taxonomyService = taxonomyService;
        }

        public BatchJob CreateRebuildIndexJob(long currentUserId, bool rebuildHistoryIndex = false)
        {
            BatchJob job = null;
            long userId = currentUserId;
            var @params = new BatchActionParameters();
            if (!rebuildHistoryIndex)
            {
                job = new BatchJob("Rebuild active indexes", userId, _batchActionFactory);
                _batchJobManager.AddAction(job, BatchActionType.RebuildSearchIndexActive, @params);
            }
            else
            {
                job = new BatchJob("Rebuild history index", userId, _batchActionFactory);
                _batchJobManager.AddAction(job, BatchActionType.RebuildSearchIndexHistory, @params);
            }
            _batchJobManager.SaveJob(job);
            return job;
        }

        public BatchJob CreateLocationMoveJob(long currentUserId, BatchActionType type, object[] args = null)
        {
            if (args == null)
                throw new System.ArgumentNullException();

            List<long> locationIds = args[0] as List<long>;
            List<long> assetTypeIDs = args[1] as List<long>;

            //List<long> LocationIds, List<long> assetTypeIDs
            var job = new BatchJob("Moving to new locaiton", currentUserId, _batchActionFactory);
            var par = new BatchActionParameters();

            string locationCVS = string.Empty;
            foreach (long locId in locationIds)
                locationCVS += locId.ToString() + ";";
            locationCVS = locationCVS.TrimEnd(new char[1] { ';' });
            par.Add("NewLocations", locationCVS);

            string uidsCVS = string.Empty;
            foreach (long uid in assetTypeIDs)
                uidsCVS += uid.ToString() + ";";
            uidsCVS = uidsCVS.TrimEnd(new char[1] { ';' });
            par.Add("AssetTypes", uidsCVS);
            _batchJobManager.AddAction(job, BatchActionType.MoveToLocation, par);
            return job;
        }

        public BatchJob CreateUpdateAssetsJob(string searchId, string typeUid, long userId, List<AttributeElement> attributeElements)
        {
            var @params = new BatchActionParameters
            {
                {"searchid", searchId},
                {"typeuid", typeUid},
                {ImportExportParameter.UserID.ToString(), userId.ToString()},
                {"attributeElements", JsonConvert.SerializeObject(attributeElements)}
            };
            var job = new BatchJob("Updating assets", userId, _batchActionFactory); 
            _batchJobManager.AddAction(job, BatchActionType.UpdateAssets, @params);
            _batchJobManager.SaveJob(job);
            return job;
        }

        public BatchJob CreateSyncAssetsJob(SyncAssetsParameters parameters)
        {
            var assetType = _assetTypeRepository.GetById(parameters.AssetTypeId);
            var job = new BatchJob(
                string.Format("Synchronizing {0} on key {1}", assetType.NameInvariant, parameters.AssetIdentifier),
                parameters.UserID,
                _batchActionFactory);
            _batchJobManager.AddAction(job, BatchActionType.SynkAssets, parameters);
            _batchJobManager.SaveJob(job);
            return job;
        }

        public BatchJob CreatePublishTypeJob(long atId, long currentUserId, bool newRevision = false)
        {
            var original = _assetTypeRepository.GetById(atId);
            var currentData = _unitOfWork.DynEntityConfigRepository
                                        .AsQueryable()
                                        .Where(dec => dec.DynEntityConfigId == atId)
                                        .OrderByDescending(dec => dec.DynEntityConfigUid)
                                        .FirstOrDefault();

            var current = new AssetType(currentData, _unitOfWork);

            string title = string.Format("Publish type ({0}) from rev. {1} to {2}", current.NameInvariant,
                                         original.Revision, current.Revision);

            var job = new BatchJob(title, currentUserId, _batchActionFactory) { SkipErrors = false };
            var par = new BatchActionParameters
                {
                    {"FromAssetType", original.UID.ToString()},
                    {"ToAssetType", current.UID.ToString()},
                    {"DynEntityConfigId", atId.ToString()}
                };
            _batchJobManager.AddAction(job, BatchActionType.PublishAssetType, par);
            if (newRevision)
            {
                _batchJobManager.AddAction(job, BatchActionType.CreateAssetsRevision, par, false);
                _batchJobManager.AddAction(job, BatchActionType.UpdateAssetsReferences, par);
                _batchJobManager.AddAction(job, BatchActionType.RecalculateAssets, par);
            }
            else
            {
                _batchJobManager.AddAction(job, BatchActionType.UpdateAssetsReferences, par);
            }

            _batchJobManager.AddAction(job, BatchActionType.RebuildReportingView, par);
            return job;
        }

        /// <summary>
        /// Imports assets from given XML file.
        /// </summary>
        /// <param name="filePath">path to file.</param>
        /// <param name="assetTypeId">ID of AssetType which assets belongs to.</param>
        /// <param name="bindings">DataSource fields bindings.</param>
        /// <param name="sheets"></param>
        /// <param name="deleteSourceOnSuccess">Should source data be deleted on successfull import or not.</param>
        /// <returns>Created BatchJob if success or null if not</returns>
        public BatchJob CreateImportAssetsJob(long currentUserId, string filePath, long assetTypeId, BindingInfo bindings, List<string> sheets, bool deleteSourceOnSuccess)
        {
            var dbEntity = new Entities.ImportExport
            {
                GUID = Guid.NewGuid(),
                FilePath = filePath,
                Bindings = bindings.ToXml(),
                Status = (int) ImportExportStatus.New,
                OperationType = (int) ImportExportOperationType.Import,
                UpdateDate = DateTime.Now,
                UpdateUserId = currentUserId
            };
            // add required extra parameters to operation
            var ap = new ImportExportParameters
            {
                {ImportExportParameter.AssetTypeId, assetTypeId.ToString()},
                {ImportExportParameter.DeleteOnSuccess, deleteSourceOnSuccess.ToString()},
                {ImportExportParameter.Sheets, string.Join(",", sheets)}
            };
            dbEntity.Parameters = ap.ToXml();

            // save the state of importing operation
            _unitOfWork.ImportExportRepository.Insert(dbEntity);
            _unitOfWork.Commit();

            if (dbEntity.GUID.Equals(Guid.Empty))
                throw new Exception("Cannot save import parameters");

            var at = _assetTypeRepository.GetById(assetTypeId);
            var job = new BatchJob(string.Format("{0} import", at.NameInvariant), currentUserId, _batchActionFactory);
            _batchJobManager.AddAction(job, BatchActionType.ImportAssets,
                new BatchActionParameters
                {
                    {ImportExportParameter.Guid.ToString(), dbEntity.GUID.ToString()},
                    {ImportExportParameter.UserID.ToString(), currentUserId.ToString()}
                });
            _batchJobManager.SaveJob(job);
            return job;
        }

        public BatchJob CreateTaxonomyJob(long _taxonomyUid, long currentUserId)
        {
            var newTaxonomy = _taxonomyService.GetByUid(_taxonomyUid);
            var oldTaxonomy = _taxonomyService.GetById(newTaxonomy.ID);

            if (newTaxonomy == null || oldTaxonomy == null)
                throw new Exception("Taxonomy not found");

            var job = new BatchJob(
                "Publishing taxonomy tree",
                currentUserId,
                _batchActionFactory);
            _batchJobManager.AddAction(job, BatchActionType.TaxonomyBatch, new BatchActionParameters
            {
                {"FromUid", oldTaxonomy.UID.ToString()},
                {"ToUid", newTaxonomy.UID.ToString()}
            });
            return job;
        }
    }
}
