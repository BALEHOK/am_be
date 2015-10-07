using System.Collections.Generic;
using AppFramework.Entities;

namespace AppFramework.Core.Classes.ScreensServices
{
    public interface IScreensService
    {
        List<AssetTypeScreen> GetScreensByAssetTypeUid(long assetTypeUid);
        AssetTypeScreen GetScreenById(long screenId);
        List<ScreenFormulaAttributeModel> GetScreenFormulaAttributes(long screenId);
        void Delete(long screenId);
        void Save(AssetTypeScreen screen);
    }
}