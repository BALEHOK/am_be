using System.Collections.Generic;

namespace AppFramework.Core.Classes
{
    public interface IAssetTemplateService
    {
        List<AssetTemplate> GetTemplatesByAssetTypeUid(long assetTypeUid);
        AssetTemplate GetTemplateByID(long assetTypeUID, long templateId);
        void Save(AssetTemplate template);
        bool Update(AssetTemplate template);
        bool Delete(AssetTemplate template);
    }
}