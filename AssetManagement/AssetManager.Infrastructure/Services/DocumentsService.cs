using System.Collections.Generic;
using AppFramework.ConstantsEnumerators;
using AssetManager.Infrastructure.Models;
using AppFramework.Core.Classes;

namespace AssetManager.Infrastructure.Services
{
    public interface IDocumentService
    {
        IEnumerable<AssetModel> GetDocuments(long userId, string query = null, int? rowStart = 1, int? rowsNumber = 20);
    }

    public class DocumentService : IDocumentService
    {
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IAssetService _assetService;

        public DocumentService(
            IAssetTypeRepository assetTypeRepository,
            IAssetService assetService)
        {
            if (assetTypeRepository == null)
                throw new System.ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;
            if (assetService == null)
                throw new System.ArgumentNullException("assetService");
            _assetService = assetService;
        }

        public IEnumerable<AssetModel> GetDocuments(long userId, string query = null, int? rowStart = 1, int? rowsNumber = 20)
        {
            var docType = _assetTypeRepository.GetPredefinedAssetType(PredefinedEntity.Document);
            return _assetService.GetAssetsByName(docType.ID, userId, query, rowStart, rowsNumber);
        }
    }
}
