using System.Collections.Generic;

namespace AssetManager.Infrastructure.Models.TypeModels
{
    public class TypesInfoModel
    {
        public List<AssetTypeModel> ActiveTypes { get; set; }

        public TypesInfoModel()
        {
            ActiveTypes = new List<AssetTypeModel>();
        }
    }
}