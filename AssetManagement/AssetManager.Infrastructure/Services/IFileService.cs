using AppFramework.ConstantsEnumerators;

namespace AssetManager.Infrastructure.Services
{
    public interface IFileService
    {
        Enumerators.MediaType GetAttributeMediaType(long assetTypeId, long attributeId);
        string GetImagesUploadDirectory(long assetTypeId, long attributeId);
        string GetReportTemplatesUploadDirectory();
        string GetDocsUploadDirectory(long assetTypeId, long attributeId);
        string GetRelativeFilepath(long assetTypeId, long attributeId, string datatype, string value);
        string GetDestinationFilename(string uploadedFilename, string uploadsDirRelPath);
        string MoveFileOnAssetCreation(
            long assetTypeId,
            long attributeId,
            long relatedAssetTypeId,
            long relatedAttributeId,
            string filename);
    }
}
