using System.Threading.Tasks;
using AssetManager.Infrastructure.Models;
using AssetManager.Infrastructure.Models.TypeModels;
using AssetManagerAdmin.Infrastructure;

namespace AssetManagerAdmin.WebApi
{
    public interface IAssetsApi
    {
        [ProgressBar("Loading asset types...")]
        Task<TypesInfoModel> GetTypesInfo();
 
        [ProgressBar("Creating a report...")]
        Task<long> CreateReport(string reportName, long assetTypeId);

        [ProgressBar("Deleting Report...")]
        Task DeleteReport(long reportId);

        [ProgressBar("Saving Formula...")]
        Task<string> SaveFormula(AssetTypeModel assetType, string attributeName, string formula);

        [ProgressBar("Saving Screen Formula...")]
        Task<string> SaveScreenFormula(ScreenPanelAttributeModel panelAttribute);

        [ProgressBar("Saving Validation...")]
        Task<string> SaveValidation(AssetTypeModel assetType, string attributeName, string expression);

        [ProgressBar("Validating Attribute...")]
        Task<AttributeValidationResultModel> ValidateAttributeAsync(long id, string value,
            string expression);

        [ProgressBar("Uploading File...")]
        Task UploadFile(byte[] file, string fileName);
    }
}