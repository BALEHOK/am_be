using System.Collections.Generic;
using System.Threading.Tasks;
using AssetManager.Infrastructure.Models;
using AssetManager.Infrastructure.Models.TypeModels;

namespace AssetManagerAdmin.WebApi
{
    public interface IAssetsApi
    {
        Task<TypesInfoModel> GetTypesInfo();
        Task<List<CustomReportModel>> GetReportsList();
        Task<string> PublishReport(string name, string fileName, long typeId);
        Task<string> DeleteReport(string name, long typeId);
        Task<string> SaveFormula(AssetTypeModel assetType, string attributeName, string formula);
        Task<string> SaveScreenFormula(ScreenPanelAttributeModel panelAttribute);
        Task<string> SaveValidation(AssetTypeModel assetType, string attributeName, string expression);

        Task<AttributeValidationResultModel> ValidateAttributeAsync(long id, string value,
            string expression);

        Task UploadFile(byte[] file, string fileName);
    }
}