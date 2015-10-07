using System;
using System.Collections.Generic;
using AssetManager.Infrastructure.Models.TypeModels;

namespace AssetManagerAdmin.Model
{
    public interface IDataService
    {
        TypesInfoModel TypesInfo { get; set; }
        UserInfo CurrentUser { get; set; }
        AssetTypeModel CurrentAssetType { get; set; }
        AttributeTypeModel CurrentAssetAttribute { get; set; }
        ServerConfig SelectedServer { get; set; }

        void GetMainMenuItems(Action<List<MainMenuItem>, Exception> callback);
        void GetTypesInfo(string server, Action<TypesInfoModel, Exception> callback);
        void GetValidationButtons(Action<List<ValidationButton>, Exception> callback);
    }
}