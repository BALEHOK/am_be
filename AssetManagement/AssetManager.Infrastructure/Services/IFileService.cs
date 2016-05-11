using AppFramework.ConstantsEnumerators;
using AssetManager.Infrastructure.Models;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace AssetManager.Infrastructure.Services
{
    public interface IFileService
    {
        Enumerators.MediaType GetAttributeMediaType(long assetTypeId, long attributeId);

        FileInfo UploadFile(IEnumerable<MultipartFileData> fileData, string uploadsDir);
        
        FileInfo MoveFileOnAssetCreation(
            long assetTypeId,
            long attributeId,
            long relatedAssetTypeId,
            long relatedAttributeId,
            string filename);

        string GetFilepath(AttributeModel attribute);
    }
}
