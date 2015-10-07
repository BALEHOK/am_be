using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes.Barcode;
using AppFramework.Core.Classes.IE;
using AppFramework.Core.Classes.IE.Adapters;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.DataProxy;
using AppFramework.Entities;
using Common.Logging;
using System;
using System.IO;
using System.Linq;

namespace AppFramework.Core.Classes.Batch.AssetActions
{
    internal class ImportAssets : BatchAction
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssetsService _assetsService;
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly ILog _logger;
        private readonly IBarcodeProvider _barcodeProvider;

        public ImportAssets(Entities.BatchAction batchAction, 
            IUnitOfWork unitOfWork, 
            IAssetsService assetsService, 
            IAssetTypeRepository assetTypeRepository,
            IBarcodeProvider barcodeProvider,
            ILog logger)
            : base(batchAction)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("IUnitOfWork");
            if (assetsService == null)
                throw new ArgumentNullException("IAssetsService");
            if (assetTypeRepository == null)
                throw new ArgumentNullException("IAssetTypeRepository");
            if (logger == null)
                throw new ArgumentNullException("ILog");
            if (barcodeProvider == null)
                throw new ArgumentNullException("barcodeProvider");
            _barcodeProvider = barcodeProvider;
            _unitOfWork = unitOfWork;
            _assetsService = assetsService;
            _assetTypeRepository = assetTypeRepository;
            _logger = logger;
        }

        public override void Run()
        {
            var importer = new AssetsImporter(
                _assetTypeRepository,
                new ExcelToXmlConverter(),
                new XMLToAssetsAdapter(
                    _assetsService,
                    _assetTypeRepository,
                    new LinkedEntityFinder(_unitOfWork),
                    _barcodeProvider),
                _logger);

            // arrange input
            var taskId = new Guid(Parameters[ImportExportParameter.Guid.ToString()].ToString());
            var userId = long.Parse(Parameters[ImportExportParameter.UserID.ToString()].ToString());
            var dbEntity = _unitOfWork.ImportExportRepository.Single(ie => ie.GUID == taskId);
            var bindings = BindingInfo.GetFromXml(dbEntity.Bindings);
            var importParams = ImportExportParameters.GetFromXml(dbEntity.Parameters);
            long assetTypeId;
            if (!long.TryParse(importParams[ImportExportParameter.AssetTypeId], out assetTypeId))
                throw new ArgumentException("AssetTypeId was not provided");
            var sheets = importParams[ImportExportParameter.Sheets].Split(new[] {','}).ToList();
            var deleteSource = importParams.ContainsKey(ImportExportParameter.DeleteOnSuccess)
                                && importParams[ImportExportParameter.DeleteOnSuccess].ToLower() == "true";
            
            // perform import
            var assets = importer.Import(assetTypeId, userId, dbEntity.FilePath, sheets, bindings);
            foreach (var asset in assets)
            {
                var at = asset.GetConfiguration();
                asset[AttributeNames.DynEntityConfigUid].Value = at.UID.ToString();
                if (asset.GetConfiguration().AutoGenerateNameType 
                    == Enumerators.TypeAutoGenerateName.InsertOnly ||
                    asset.GetConfiguration().AutoGenerateNameType 
                    == Enumerators.TypeAutoGenerateName.InsertUpdate)
                    asset[AttributeNames.Name].Value = asset.GenerateName();
                _assetsService.InsertAsset(asset);
            }

            // update import/export task entry   
            //dbEntity.Status = result.IsSuccess 
            //    ? (int)ImportExportStatus.Completed 
            //    : (int)ImportExportStatus.Error;
            //if (result.Errors.Count > 0)
            //    dbEntity.Message += "\r\nErrors:\r\n" + string.Join("\r\n", result.Errors.ToArray());
            //if (result.Warnings.Count > 0)
            //    dbEntity.Message += "\r\nWarnings:\r\n" + string.Join("\r\n", result.Warnings.ToArray());
            _unitOfWork.ImportExportRepository.Update(dbEntity);
            _unitOfWork.Commit();

            // cleanup
            if (deleteSource)
                File.Delete(dbEntity.FilePath);
        }
    }
}
