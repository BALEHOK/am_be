using System.Collections.Generic;
using AssetManager.Infrastructure.Models.TypeModels;
using System.Threading.Tasks;

namespace AssetManagerAdmin.Model
{
    public interface IDataService
    {
        AssetTypeModel CurrentAssetType { get; set; }

        AttributeTypeModel CurrentAssetAttribute { get; set; }

        List<MainMenuItem> GetMainMenuItems(UserInfo user);
                
        Task<TypesInfoModel> GetTypesInfo(UserInfo user, string server);

        List<ValidationButton> GetValidationButtons();
    }
}